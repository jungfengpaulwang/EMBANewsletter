using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using PerceptiveMCAPI.Types;
using PerceptiveMCAPI;
using PerceptiveMCAPI.Methods;
using FISCA.UDT;
using System.Threading.Tasks;
using FISCA.Data;
using System.Xml.Linq;

namespace Newsletter.Forms
{
    public partial class frmSyncSubscriber : BaseForm
    {
        private string MailChimp_API_Key;
        private AccessHelper Access;
        private QueryHelper Query;
        private Dictionary<string, McList> dicMailChimpLists;
        private Dictionary<string, dynamic> SelectedItems;
        private Dictionary<string, List<string>> TeacherTags;
        private Dictionary<string, List<string>> StudentTags;
        private Dictionary<string, dynamic> dicTeachers;
        private Dictionary<string, dynamic> dicStudents;
        private List<dynamic> StudentNewsletterGroups;
        private List<dynamic> TeacherNewsletterGroups;
        private bool form_loaded;

        public frmSyncSubscriber()
        {
            InitializeComponent();
            
            this.MailChimp_API_Key = string.Empty;
            this.dicMailChimpLists = new Dictionary<string, McList>();
            this.SelectedItems = new Dictionary<string, dynamic>();
            this.TeacherTags = new Dictionary<string, List<string>>();
            this.StudentTags = new Dictionary<string, List<string>>();
            this.dicTeachers = new Dictionary<string, dynamic>();
            this.dicStudents = new Dictionary<string, dynamic>();
            this.StudentNewsletterGroups = new List<dynamic>();
            this.TeacherNewsletterGroups = new List<dynamic>();

            this.Access = new AccessHelper();
            this.Query = new QueryHelper();

            this.Load += new EventHandler(Form_Load);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.circularProgress.Visible = false;
            this.circularProgress.IsRunning = false;
            this.form_loaded = false;

            List<UDT.MailChimpAPIKey> MailChimpAPIKeys = Access.Select<UDT.MailChimpAPIKey>();
            if (MailChimpAPIKeys.Count > 0)
            {
                this.MailChimp_API_Key = MailChimpAPIKeys.ElementAt(0).APIKey;
            }
            else
            {
                MessageBox.Show("請先設定「MailChimp API Key」。");
                this.Close();
            }

            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;

            Task task = Task.Factory.StartNew(() =>
            {
                this.StudentNewsletterGroups = this.GetStudentNewsletterGroups();
                this.TeacherNewsletterGroups = this.GetTeacherNewsletterGroups();
            });
            task.ContinueWith((x) =>
            {
                if (x.Exception != null)
                {
                    MessageBox.Show(x.Exception.InnerException.Message);
                    this.circularProgress.IsRunning = false;
                    this.circularProgress.Visible = false;
                    this.form_loaded = true;
                    return;
                }
                foreach (dynamic o in this.StudentNewsletterGroups.Union(this.TeacherNewsletterGroups))
                {
                    ListViewItem item = new ListViewItem();

                    item.Text = o.TagName;
                    item.Tag = o;
                    item.Checked = true;

                    if (!this.SelectedItems.ContainsKey(item.Text))
                        this.SelectedItems.Add(item.Text, item.Tag);

                    this.FieldContainer.Items.Add(item);
                }
                this.circularProgress.IsRunning = false;
                this.circularProgress.Visible = false;
                this.form_loaded = true;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            if (this.SelectedItems.Count == 0)
            {
                MessageBox.Show("請先選擇電子報名單。");
                return;
            }

            listsInput input = new listsInput(this.MailChimp_API_Key);
            lists cmd = new lists();
            listsOutput output = cmd.Execute(input);
            List<string> lists_duplicated = new List<string>();
            this.dicMailChimpLists.Clear();
            if (output != null)
            {
                if (output.result != null)
                {
                    foreach (var listsResult in output.result)
                    {
                        if (!dicMailChimpLists.ContainsKey(listsResult.name))
                        {
                            dicMailChimpLists.Add(listsResult.name, new McList()
                            {
                                ListId = listsResult.id,
                                ListName = listsResult.name,
                                MemberCount = listsResult.member_count,
                                UnsubscribeCount = listsResult.unsubscribe_count
                            });
                        }
                        else
                        {
                            if (this.SelectedItems.ContainsKey(listsResult.name))
                                lists_duplicated.Add(listsResult.name);
                        }
                    }
                }
            }
            lists_duplicated = lists_duplicated.Distinct().ToList();
            if (lists_duplicated.Count > 0)
            {
                MessageBox.Show(string.Format("下列 MailChimp Lists 名稱重覆，請先移除重覆之 MailChimp Lists：\n\n{0}", string.Join(",", lists_duplicated.Select(x => x))));
                return;
            }
            List<string> lists_NoneMailChimp = new List<string>();
            this.SelectedItems.Keys.ToList().ForEach((x) =>
            {
                if (!this.dicMailChimpLists.ContainsKey(x))
                    lists_NoneMailChimp.Add(x);
            });
            if (lists_NoneMailChimp.Count > 0)
            {
                MessageBox.Show(string.Format("下列電子報名單不存在於 MailChimp Lists，請先在 MailChimp 中建立：\n\n{0}", string.Join(",", lists_NoneMailChimp.Select(x => x))));
                return;
            }

            this.circularProgress.Visible = true;
            this.circularProgress.IsRunning = true;
            Task task = Task.Factory.StartNew(() =>
            {
                this.TeacherTags = this.GetTeacherTags();
                this.StudentTags = this.GetStudentTags();

                List<string> TeacherIDs = new List<string>();
                List<string> StudentIDs = new List<string>();
                this.TeacherTags.Values.ToList().ForEach(x => TeacherIDs.AddRange(x));
                this.StudentTags.Values.ToList().ForEach(x => StudentIDs.AddRange(x));

                this.dicTeachers = this.GetTeachersByIDs(TeacherIDs.Distinct().ToList());
                this.dicStudents = this.GetStudentsByIDs(StudentIDs.Distinct().ToList());

                foreach (string key in this.SelectedItems.Keys)
                {
                    listBatchSubscribe_method(this.SelectedItems[key]);
                }             
            });
            task.ContinueWith((x) =>
            {
                if (x.Exception != null)
                    MessageBox.Show(x.Exception.InnerException.Message);
                else
                    MessageBox.Show("同步完成。");

                this.circularProgress.IsRunning = false;
                this.circularProgress.Visible = false;
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private List<string> GetListSubscriberEMails(string apikey, string id)
        {
            listMembersInput input_member = new listMembersInput();
            input_member.parms.apikey = apikey;
            input_member.parms.id = id;
            input_member.parms.start = 0;
            input_member.parms.limit = 15000;
            input_member.parms.status = EnumValues.listMembers_status.cleaned | EnumValues.listMembers_status.NotSpecified | EnumValues.listMembers_status.subscribed | EnumValues.listMembers_status.unsubscribed | EnumValues.listMembers_status.updated;
            listMembers cmd = new listMembers(input_member);
            listMembersOutput output_member = cmd.Execute();

            List<string> EMails = new List<string>();
            output_member.result.ForEach(x => EMails.Add(x.email));

            return EMails;
        }

        private listBatchUnsubscribeOutput listBatchUnSubscribe_method(string apikey, string id, List<string> EMails)
        {
            listBatchUnsubscribeInput input = new listBatchUnsubscribeInput();
            input.parms.apikey = apikey;
            input.parms.delete_member = true;
            input.parms.emails = EMails;
            input.parms.id = id;
            input.parms.send_goodbye = false;
            input.parms.send_notify = false;
            listBatchUnsubscribe cmd_Unsubscribe = new listBatchUnsubscribe(input);
            listBatchUnsubscribeOutput output = cmd_Unsubscribe.Execute();
            return output;
        }

        public void listBatchSubscribe_method(dynamic o)
        {
            string TagID = o.TagID;
            string TagName = o.TagName;
            string Category = o.Category;

            if (!dicMailChimpLists.ContainsKey(TagName))
                return;

            string MailChimp_List_ID = dicMailChimpLists[TagName].ListId;
            List<string> EMails = this.GetListSubscriberEMails(this.MailChimp_API_Key, MailChimp_List_ID).Distinct().ToList();
            Dictionary<string, string> dicEMails = new Dictionary<string, string>();
            if (EMails.Count > 0)
                dicEMails = EMails.ToDictionary(x => x);

            listBatchSubscribeInput input = new listBatchSubscribeInput();
            // any directive overrides
            input.api_Validate = true;
            input.api_AccessType = EnumValues.AccessType.Serial;
            input.api_OutputType = EnumValues.OutputType.XML;
            // method parameters
            input.parms.apikey = this.MailChimp_API_Key;
            input.parms.id = MailChimp_List_ID;
            input.parms.double_optin = false;
            input.parms.replace_interests = true;
            input.parms.update_existing = true;
            
            List<Dictionary<string, object>> batch = new List<Dictionary<string, object>>();
            if (Category == "Student")
            {
                List<string> StudentIDs = new List<string>();
                if (this.StudentTags.ContainsKey(TagID))
                    StudentIDs = this.StudentTags[TagID];
                List<dynamic> Students = new List<dynamic>();
                StudentIDs.ForEach(x=>
                {
                    if (dicStudents.ContainsKey(x))
                        Students.Add(dicStudents[x]);
                });
                SyncListMergeVars(this.MailChimp_API_Key, MailChimp_List_ID, new Dictionary<string, string>() { { "ID", "學生系統編號" }, { "NAME", "姓名" }, { "GENDER", "性別" }, { "BIRTHDAY", "生日" }, { "YEAR", "生日之年" }, { "MONTH", "生日之月" }, { "DAY", "生日之日" }, { "ENROLLYEAR", "入學年度" }, { "DEPT", "系所組別" }, { "EMAIL", "電子郵件" }, { "STATUS", "學生狀態" } });
                foreach (dynamic oo in Students)
                {
                    if (oo.Status == "退學" || oo.Status == "刪除")
                        continue;

                    foreach (string email in oo.EMails)
                    {
                        Dictionary<string, object> entry = new Dictionary<string, object>();

                        entry.Add("ID", oo.ID);
                        entry.Add("NAME", oo.Name);
                        entry.Add("GENDER", oo.Gender);
                        entry.Add("BIRTHDAY", oo.Birthday);
                        entry.Add("YEAR", oo.Birthday_Year);
                        entry.Add("MONTH", oo.Birthday_Month);
                        entry.Add("DAY", oo.Birthday_Day);
                        entry.Add("ENROLLYEAR", oo.EnrollYear);
                        entry.Add("DEPT", oo.Dept);
                        entry.Add("EMAIL", email);
                        entry.Add("STATUS", oo.Status);

                        batch.Add(entry);
                        if (dicEMails.ContainsKey(email))
                            dicEMails.Remove(email);
                    }
                }
            }
            else if (Category == "Teacher")
            {
                List<string> TeacherIDs = new List<string>();
                if (this.TeacherTags.ContainsKey(TagID))
                    TeacherIDs = this.TeacherTags[TagID];
                List<dynamic> Teachers = new List<dynamic>();
                TeacherIDs.ForEach(x =>
                {
                    if (this.dicTeachers.ContainsKey(x))
                        Teachers.Add(dicTeachers[x]);
                });
                SyncListMergeVars(this.MailChimp_API_Key, MailChimp_List_ID, new Dictionary<string, string>() { { "ID", "教師系統編號" }, { "NAME", "姓名" }, { "GENDER", "性別" }, { "BIRTHDAY", "生日" }, { "YEAR", "生日之年" }, { "MONTH", "生日之月" }, { "DAY", "生日之日" }, { "EMAIL", "電子郵件" } });
                foreach (dynamic oo in Teachers)
                {
                    if (oo.Status == "256")
                        continue;

                    foreach (string email in oo.EMails)
                    {
                        Dictionary<string, object> entry = new Dictionary<string, object>();

                        entry.Add("ID", oo.ID);
                        entry.Add("NAME", oo.Name);
                        entry.Add("GENDER", oo.Gender);
                        entry.Add("BIRTHDAY", oo.Birthday);
                        entry.Add("YEAR", oo.Birthday_Year);
                        entry.Add("MONTH", oo.Birthday_Month);
                        entry.Add("DAY", oo.Birthday_Day);
                        entry.Add("EMAIL", email);

                        batch.Add(entry);
                        if (dicEMails.ContainsKey(email))
                            dicEMails.Remove(email);
                    }
                }
            }
            input.parms.batch = batch;

            //  取消訂閱不在電子報名單中的訂閱者 
            List<string> UnSubscribeEMails = new List<string>();
            foreach (string email in dicEMails.Keys)
                UnSubscribeEMails.Add(email);

            listBatchUnsubscribeOutput output_unsubscribe = this.listBatchUnSubscribe_method(this.MailChimp_API_Key, MailChimp_List_ID, UnSubscribeEMails);

            // 訂閱電子報
            listBatchSubscribe cmd = new listBatchSubscribe(input);
            listBatchSubscribeOutput output = cmd.Execute();
            // output, format with user control
            //if (output.api_ErrorMessages.Count > 0)
            //{
            //    showResults(output.api_Request, output.api_Response, // raw data
            //    output.api_ErrorMessages, output.api_ValidatorMessages); // & errors
            //}
            //else
            //{
            //    show_listBatch1.Display(output);
            //}
        }

        //取得學生「電子報名單」類別的子類別
        private List<dynamic> GetStudentNewsletterGroups()
        {
            string cmd = @"select id, name from tag where category='Student' and prefix='電子報名單' order by category, name, id";
            List<dynamic> NewsletterGroups = new List<dynamic>();
            DataTable dt = Query.Select(cmd);
            foreach (DataRow row in dt.Rows)
                NewsletterGroups.Add(new { TagID = row["id"] + "", TagName = row["name"] + "", Category = "Student" });

            return NewsletterGroups;
        }

        //取得教師「電子報名單」類別的子類別
        private List<dynamic> GetTeacherNewsletterGroups()
        {
            string cmd = @"select id, name from tag where category='Teacher' and prefix='電子報名單' order by category, name, id";
            List<dynamic> NewsletterGroups = new List<dynamic>();
            DataTable dt = Query.Select(cmd);
            foreach (DataRow row in dt.Rows)
                NewsletterGroups.Add(new { TagID = row["id"] + "", TagName = row["name"] + "", Category = "Teacher" });

            return NewsletterGroups;
        }

        //取得教師與 tag 的關聯
        private Dictionary<string, List<string>> GetTeacherTags()
        {
            string cmd = @"SELECT ref_teacher_id, ref_tag_id from tag_teacher ";
            Dictionary<string, List<string>> teacher_tags = new Dictionary<string, List<string>>();

            DataTable dt = Query.Select(cmd);
            foreach (DataRow row in dt.Rows)
            {
                string tagID = row["ref_tag_id"].ToString();
                if (!teacher_tags.ContainsKey(tagID))
                    teacher_tags.Add(tagID, new List<string>());

                string teacherID = row["ref_teacher_id"].ToString();
                teacher_tags[tagID].Add(teacherID);
            }

            return teacher_tags;
        }

        //取得學生與 tag 的關聯
        private Dictionary<string, List<string>> GetStudentTags()
        {
            string cmd = @"SELECT ref_student_id, ref_tag_id from tag_student";
            Dictionary<string, List<string>> student_tags = new Dictionary<string, List<string>>();

            DataTable dt = Query.Select(cmd);
            foreach (DataRow row in dt.Rows)
            {
                string tagID = row["ref_tag_id"].ToString();
                if (!student_tags.ContainsKey(tagID))
                    student_tags.Add(tagID, new List<string>());

                string studentID = row["ref_student_id"].ToString();
                student_tags[tagID].Add(studentID);
            }

            return student_tags;
        }

        //取得所有教師資料
        private Dictionary<string, dynamic> GetTeachersByIDs(List<string> IDs)
        {
            string cmd = string.Format(@"select teacher.id, teacher_name, Case gender when '1' then '男' when '0' then '女' else '' end as gender, te.birthday, te.major_work_place, email, teacher.status from teacher left join $ischool.emba.teacher_ext as te on teacher.id=te.ref_teacher_id where id in ({0})", string.Join(",", IDs));

            Dictionary<string, dynamic> dicTeachers = new Dictionary<string, dynamic>();
            DataTable dt = Query.Select(cmd);
            foreach (DataRow row in dt.Rows)
            {
                if (!dicTeachers.ContainsKey(row["id"] + ""))
                {
                    DateTime birthday;
                    string strBirthday = (row["birthday"] + "").Trim();
                    string strBirthday_Year = string.Empty;
                    string strBirthday_Month = string.Empty;
                    string strBirthday_Day = string.Empty;
                    if (DateTime.TryParse(strBirthday, out birthday))
                    {
                        strBirthday = birthday.ToShortDateString();
                        strBirthday_Year = birthday.Year.ToString();
                        strBirthday_Month = birthday.Month.ToString();
                        strBirthday_Day = birthday.Day.ToString();
                    }
                    List<string> emails = new List<string>();
                    emails = (row["email"] + "").Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x=>x.Trim()).Distinct().ToList();
                    dicTeachers.Add(row["id"] + "", new { ID = row["id"] + "", Name = (row["teacher_name"] + "").Trim(), Gender = row["gender"] + "", Birthday = strBirthday, Birthday_Year = strBirthday_Year, Birthday_Month = strBirthday_Month, Birthday_Day = strBirthday_Day, MajorWorkPlace = row["major_work_place"] + "", EMails = emails, Status = row["status"] + "" });
                }
            }

            return dicTeachers;
        }

        //  取得所有學生資料
        private Dictionary<string, dynamic> GetStudentsByIDs(List<string> IDs)
        {
            string cmd = string.Format(@"select student.id as student_id, student.name as student_name, Case gender when '1' then '男' when '0' then '女' else '' end as gender, birthdate, sb.enroll_year, dg.name as dept, sa_login_name, sb.email_list, student.status from student 
left join $ischool.emba.student_brief2 as sb on sb.ref_student_id=student.id
left join $ischool.emba.department_group as dg on dg.uid=sb.ref_department_group_id where student.id in ({0})", string.Join(",", IDs));

            Dictionary<string, dynamic> dicStudents = new Dictionary<string, dynamic>();
            DataTable dt = Query.Select(cmd);
            foreach (DataRow row in dt.Rows)
            {
                if (!dicStudents.ContainsKey(row["student_id"] + ""))
                {
                    DateTime birthday;
                    string strBirthday = (row["birthdate"] + "").Trim();
                    string strBirthday_Year = string.Empty;
                    string strBirthday_Month = string.Empty;
                    string strBirthday_Day = string.Empty;
                    if (DateTime.TryParse(strBirthday, out birthday))
                    {
                        strBirthday = birthday.ToShortDateString();
                        strBirthday_Year = birthday.Year.ToString();
                        strBirthday_Month = birthday.Month.ToString();
                        strBirthday_Day = birthday.Day.ToString();
                    }
                    List<string> emails = new List<string>();
                    if (!string.IsNullOrWhiteSpace(row["sa_login_name"] + ""))
                        emails.Add((row["sa_login_name"] + "").Trim());

                    string emailList = "<?xml version='1.0' encoding='utf-8' ?><emails><email1 /><email2 /><email3 /><email4 /><email5 /></emails>";
                    if (!string.IsNullOrWhiteSpace(row["email_list"] + ""))
                        emailList = "<?xml version='1.0' encoding='utf-8' ?><emails>" + (row["email_list"] + "").Trim() + "</emails>";

                    XDocument xDocument = XDocument.Parse(emailList, LoadOptions.None);
                    IEnumerable<XElement> xElements = xDocument.Element("emails").Descendants();
                    foreach (XElement xElement in xElements)
                    {
                        if (!string.IsNullOrWhiteSpace(xElement.Value))
                            emails.Add(xElement.Value);
                    }
                    emails = emails.Distinct().ToList();

                    string strStatus = t_Status(row["status"] + "");
                    dicStudents.Add(row["student_id"] + "", new { ID = row["student_id"] + "", Name = (row["student_name"] + "").Trim(), Gender = row["gender"] + "", Birthday = strBirthday, Birthday_Year = strBirthday_Year, Birthday_Month = strBirthday_Month, Birthday_Day = strBirthday_Day, EnrollYear = row["enroll_year"] + "", Dept = row["dept"] + "", EMails = emails, Status = strStatus });
                }
            }

            return dicStudents;
        }

        private string t_Status(string status)
        {
            string t_Status = string.Empty;
            switch (status)
            {
                case "1":
                    t_Status = "在學";
                    break;
                case "4":
                    t_Status = "休學";
                    break;
                case "64":
                    t_Status = "退學";
                    break;
                case "16":
                    t_Status = "畢業";
                    break;
                case "256":
                    t_Status = "刪除";
                    break;
                default:
                    t_Status = "";
                    break;
            }
            return t_Status;
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!form_loaded)
                return;

            this.SelectedItems.Clear();
            foreach (ListViewItem item in this.FieldContainer.Items)
            {
                item.Checked = chkSelectAll.Checked;
                if (item.Checked)
                    if (!this.SelectedItems.ContainsKey(item.Text))
                        this.SelectedItems.Add(item.Text, item.Tag);
            }
        }

        private void FieldContainer_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!form_loaded)
                return;

            if (e.Item.Checked)
            {
                if (!this.SelectedItems.ContainsKey(e.Item.Text))
                    this.SelectedItems.Add(e.Item.Text, e.Item.Tag);
            }
            else
            {
                if (this.SelectedItems.ContainsKey(e.Item.Text))
                    this.SelectedItems.Remove(e.Item.Text);
            }
        }
    
        private List<string> GetListMergeVars(string apikey, string id)
        {
            listMergeVarsInput input = new listMergeVarsInput();
            input.api_Validate = true;
            input.api_AccessType = EnumValues.AccessType.Serial;
            input.api_OutputType = EnumValues.OutputType.XML;
            input.parms.apikey = apikey;
            input.parms.id = id;
            listMergeVars list = new listMergeVars();
            listMergeVarsOutput output = list.Execute(input);

            List<string> result = new List<string>();
            if (output == null)
                return result;
            if (output.result == null)
                return result;

            output.result.ForEach(x => result.Add(x.tag));
            return result;
        }

        private void AddListMergerVars(string apikey, string id, KeyValuePair<string, string> KVs)
        {
            listMergeVarAddInput input = new listMergeVarAddInput();
            input.api_Validate = true;
            input.api_AccessType = EnumValues.AccessType.Serial;
            input.api_OutputType = EnumValues.OutputType.XML;
            input.parms.apikey = apikey;
            input.parms.id = id;
            input.parms.tag = KVs.Key;
            input.parms.name = KVs.Value;
            listMergeVarAdd list = new listMergeVarAdd();
            listMergeVarAddOutput output = list.Execute(input);
        }

        private void SyncListMergeVars(string apikey, string id, Dictionary<string, string> dicVars)
        {
            List<string> oMergeVars = this.GetListMergeVars(apikey, id);

            dicVars.Keys.ToList().ForEach((x) =>
            {
                if (!oMergeVars.Contains(x))
                    this.AddListMergerVars(apikey, id, new KeyValuePair<string, string>(x, dicVars[x]));
            });
        }
    }

    public class McList
    {
        public string ListId { get; set; }
        public string ListName { get; set; }
        public int MemberCount { get; set; }
        public int UnsubscribeCount { get; set; }
    }
}
