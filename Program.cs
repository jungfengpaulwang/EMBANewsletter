using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.UDT;
using FISCA.Authentication;
using FISCA.Presentation;
using FISCA.Permission;

namespace Newsletter
{
    public class Program
    {
        [MainMethod("Newsletter")]
        public static void Main()
        {
            SyncDBSchema();
            InitDetailContent();
        }

        public static void SyncDBSchema()
        {
            #region 模組啟用先同步Schema

            SchemaManager Manager = new SchemaManager(DSAServices.DefaultConnection);

            Manager.SyncSchema(new UDT.MailChimpAPIKey());

            #endregion
        }

        public static void InitDetailContent()
        {
            #region 教務作業>電子報名單>設定>MailChimpAPIKey
            RoleAclSource.Instance["教務作業"]["功能按鈕"].Add(new RibbonFeature("Button_Newsletter_SetMailChimpAPIKey", "設定 MailChimpAPIKey"));

            MotherForm.RibbonBarItems["教務作業", "電子報名單"]["設定"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["教務作業", "電子報名單"]["設定"].Image = Properties.Resources.network_lock_64;
            MotherForm.RibbonBarItems["教務作業", "電子報名單"]["設定"]["MailChimpAPIKey"].Enable = UserAcl.Current["Button_Newsletter_SetMailChimpAPIKey"].Executable;
            MotherForm.RibbonBarItems["教務作業", "電子報名單"]["設定"]["MailChimpAPIKey"].Click += delegate
            {
                (new Forms.frmMailChimpApiKey()).ShowDialog();
            };
            #endregion

            #region 教務作業>電子報名單>管理>名單
            RoleAclSource.Instance["教務作業"]["功能按鈕"].Add(new RibbonFeature("Button_Newsletter_MailChimpListsManagement", "管理 MailChimp 電子報名單"));

            MotherForm.RibbonBarItems["教務作業", "電子報名單"]["管理"].Size = RibbonBarButton.MenuButtonSize.Large;
            MotherForm.RibbonBarItems["教務作業", "電子報名單"]["管理"].Image = Properties.Resources.sandglass_unlock_64;
            MotherForm.RibbonBarItems["教務作業", "電子報名單"]["管理"]["名單"].Enable = UserAcl.Current["Button_Newsletter_MailChimpListsManagement"].Executable;
            MotherForm.RibbonBarItems["教務作業", "電子報名單"]["管理"]["名單"].Click += delegate
            {
                (new Forms.frmSyncSubscriber()).ShowDialog();
            };
            #endregion
        }
    }
}