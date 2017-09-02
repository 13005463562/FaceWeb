using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

/// <summary>
/// DealWithFiles 的摘要说明
/// </summary>
public class DealWithFiles
{
    public DealWithFiles()
    {

    }
    /// <summary>
    /// 文件删除
    /// </summary>
    /// <param name="strPath"></param>
    /// <returns></returns>
    public bool DeleteFile(string strPath)
    {
        bool isSuccess = false;
        try
        {
            if (File.Exists(strPath))
            {
                File.Delete(strPath);
            }
            isSuccess = true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        return isSuccess;
    }
}
