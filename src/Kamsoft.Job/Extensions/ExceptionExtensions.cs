using System;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Kamsoft.Job
{
    /// <summary>
    /// 异常操作扩展
    /// </summary>
    public static class ExceptionExtensions
    {
        #region 获取最底层异常 GetDeepestException
        /// <summary>
        /// 获取最底层异常
        /// </summary>
        public static Exception GetDeepestException(this Exception ex)
        {
            var innerException = ex.InnerException;
            var resultExcpetion = ex;
            while (innerException != null)
            {
                resultExcpetion = innerException;
                innerException = innerException.InnerException;
            }
            return resultExcpetion;
        }
        #endregion

        #region 格式化异常消息 FormatMessage
        /// <summary>
        /// 格式化异常消息
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="isHideStackTrace">是否隐藏异常规模信息</param>
        /// <returns>格式化后的异常信息字符串</returns>
        public static string FormatMessage(this Exception ex, bool isHideStackTrace = false)
        {
            StringBuilder sb = new StringBuilder();
            int count = 0;
            string appString = string.Empty;
            while (ex != null)
            {
                if (count > 0)
                {
                    appString += $"    ";
                }
                sb.AppendLine(string.Format("{0}异常消息(Message)：{1}", appString, ex.Message));
                sb.AppendLine(string.Format("{0}异常类型(Type)：{1}", appString, ex.GetType().FullName));
                sb.AppendLine(string.Format("{0}异常方法(Method)：{1}", appString, (ex.TargetSite == null ? null : ex.TargetSite.Name)));
                sb.AppendLine(string.Format("{0}异常来源(Source)：{1}", appString, ex.Source));
                if (!isHideStackTrace && ex.StackTrace != null)
                {
                    sb.AppendLine(string.Format("{0}异常堆栈(StackTrace)：{1}", appString, ex.StackTrace));
                }
                if (ex.InnerException != null)
                {
                    sb.AppendLine(string.Format("{0}内部异常(InnerException)：", appString));
                    count++;
                }
                ex = ex.InnerException;
            }
            return sb.ToString();
        }
        #endregion

        #region 将异常重新抛出 ReThrow
        /// <summary>
        /// 将异常重新抛出
        /// </summary>
        public static void ReThrow(this Exception exception)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
        #endregion

        #region 抛出异常 ThrowIf
        /// <summary>
        /// 如果条件成立，则抛出异常
        /// </summary>
        public static void ThrowIf(this Exception exception, bool isThrow)
        {
            if (isThrow)
            {
                throw exception;
            }
        }
        #endregion

        #region 抛出异常 ThrowIf
        /// <summary>
        /// 如果条件成立，则抛出异常
        /// </summary>
        public static void ThrowIf(this Exception exception, Func<bool> isThrowFunc)
        {
            if (isThrowFunc())
            {
                throw exception;
            }
        } 
        #endregion
    }
}




