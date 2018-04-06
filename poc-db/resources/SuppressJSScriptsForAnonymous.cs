using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Aberto.WebControls {

    /// <summary>
    /// 
    /// </summary>
    [ToolboxData("<{0}:SuppressJSScriptsForAnonymous runat=\"server\" />")]
    public class SuppressJSScriptsForAnonymous : Control {
        private List<string> files = new List<string>();
        private List<int> indiciesOfFilesToBeRemoved = new List<int>();

        public string HttpList {
            get;
            set;
        }

        public string FilesToSuppress {
            get;
            set;
        }

        protected override void OnInit(EventArgs e) {
            files.AddRange(FilesToSuppress.Split(';'));

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e) {
            // only process if user is anonymous..
            if (!(this.Page.User.Identity.IsAuthenticated && Page.User.Identity.Name.Contains("\\"))) {
                // get list of registered script files which will be loaded..
                object oFiles = HttpContext.Current.Items[HttpList];
                if (object.ReferenceEquals(oFiles, null))
                    return;
                IList registeredFiles = (IList)oFiles;
                int i = 0;

                foreach (var file in registeredFiles) {
                    // use reflection to get the ScriptLinkInfo.Filename property, then check if in FilesToSuppress list and remove from collection if so..
                    Type t = file.GetType();
                    PropertyInfo prop = t.GetProperty("Filename");
                    if (prop != null) {
                        string filename = prop.GetValue(file, null).ToString();

                        if (!string.IsNullOrEmpty(files.Find(delegate(string sFound) {
                            return filename.ToLower().Contains(sFound.ToLower());
                        }))) {
                            indiciesOfFilesToBeRemoved.Add(i);
                        }
                    }

                    i++;
                }

                int iRemoved = 0;
                foreach (int j in indiciesOfFilesToBeRemoved) {
                    registeredFiles.RemoveAt(j - iRemoved);
                    iRemoved++;
                }

                // overwrite cached value with amended collection.. 
                HttpContext.Current.Items[HttpList] = registeredFiles;
            }

            base.OnPreRender(e);
        }
    }
}
