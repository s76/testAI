using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace React
{
    [ System.Serializable ]
    public class MethodParams
    {                
        public List<string>     ParamsValues;
        private MethodInfo      m_methodInfo;
        private object[]        m_convertedParams;   

        public MethodParams()
        {
            ParamsValues = new List<string>();
        }

        public void ReInit(MethodInfo _methodInfo)
        {
            if (m_methodInfo == null || _methodInfo != m_methodInfo)
            {
                m_methodInfo = _methodInfo;

                ParameterInfo[] mParams = _methodInfo.GetParameters();
                if (ParamsValues.Count != mParams.Length)
                {
                    ParamsValues = new List<string>(mParams.Length);
                    for (int i = 0; i < mParams.Length; ++i)
                    {
                        ParamsValues.Add(string.Empty);
                    }
                }               
            }
        }

        public void ConvertParams( MethodInfo _methodInfo )
        {
            m_convertedParams = MethodParamsConverter.ConvertParams( _methodInfo, ParamsValues );
        }

        public object[] ConvertedParams
        {
            get { return m_convertedParams;  }
            set { m_convertedParams = value;  }
        }

    }

    public static class MethodParamsConverter
    {
        public static IEnumerator<NodeResult> InvokeMethod( MethodInfo _methodInfo, Component _component, MethodParams _params )
        {
            object[] callParams = ConvertParams( _methodInfo, _params );            
            IEnumerator<NodeResult> result = (IEnumerator<NodeResult>)_methodInfo.Invoke(_component, callParams );
            return result;
        }

        public static object[] ConvertParams(MethodInfo _methodInfo, List< string > _params )
        {            
            if (_methodInfo.GetParameters().Length == null || _params.Count == 0)
            {
                return null;
            }

            ParameterInfo[] methodParams = _methodInfo.GetParameters();
            if (methodParams.Length != _params.Count)
            {
                return null;
            }
            object[] retParams = new object[methodParams.Length];
            for (int i = 0; i < methodParams.Length; ++i)
            {
                retParams[i] = SafeConvertToGeneric(_params[i], methodParams[i].ParameterType);
            }

            return retParams;
        }

        private static object[] ConvertParams(MethodInfo _methodInfo, MethodParams _params )
        {
            if (_params == null)
            {
                return null;
            }

            return ConvertParams( _methodInfo, _params.ParamsValues );
        }
        
        public static object SafeConvertToGeneric( string _paramValue, Type _type )
        {            
            try
            {
                return Convert.ChangeType( _paramValue, _type );
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert '{0}' to a '{1}'.", _paramValue, _type);
                return Activator.CreateInstance(_type);                
            }            
        }

        public static bool SafeConvertToBool(string _param)
        {
            bool currentVal = false;
            try
            {
                currentVal = Convert.ToBoolean(_param);
            }
            catch (FormatException)
            {
                Console.WriteLine("Unable to convert '{0}' to a Boolean.", _param);
            }
            return currentVal;
        }
    }
}
