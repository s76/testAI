using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;

namespace React
{
    static class ReactParamsDrawer
    {
        public static void DrawParamsInspector( MethodInfo methodInfo, MethodParams methodParams )
        {
            if (methodInfo != null && methodInfo.GetParameters().Length > 0)
            {
                GUILayout.BeginVertical();
                List< string > paramsValues = methodParams.ParamsValues;
                ParameterInfo[] paramsDef = methodInfo.GetParameters();               
                for (int i = 0; i < paramsDef.Length; ++i)
                {
                    ParameterInfo parameter = paramsDef[i];
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(parameter.Name, GUILayout.Width(100));
                    if( parameter.ParameterType == typeof( bool ) )
                    {
                        bool currentVal = MethodParamsConverter.SafeConvertToBool(paramsValues[i]);
                        paramsValues[i] =  GUILayout.Toggle( currentVal, "").ToString();
                    }
                    else if( parameter.ParameterType == typeof( string ) )
                    {
                        paramsValues[ i ] = GUILayout.TextField( paramsValues[ i ], GUILayout.Width( 120 ) ); 

                    }
                    else if( parameter.ParameterType == typeof( int ) || parameter.ParameterType == typeof( float ) )
                    {
                        string regex = "";
                        if( parameter.ParameterType == typeof( int ) )
                        {
                            regex = "[^0-9]";
                        }
                        else
                        {
                            regex = "[^.0-9]";
                        }
                        string newVal = GUILayout.TextField(paramsValues[i], GUILayout.Width(120));
                        paramsValues[i] = Regex.Replace(newVal, regex, "");                                                
                    }
                    

                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
        }
        
    }
}
