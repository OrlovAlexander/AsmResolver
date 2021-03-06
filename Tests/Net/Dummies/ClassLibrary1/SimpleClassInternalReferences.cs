﻿using System;

namespace ClassLibrary1
{
    public class SimpleClassInternalReferences
    {
        private readonly string _defaultPrefix;
        
        public SimpleClassInternalReferences()
        {
            _defaultPrefix = "abc";
        }

        public string SomeMethod(int x)
        {
            try
            {
                return SomeMethod(_defaultPrefix, x);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        
        public string SomeMethod(string prefix, int x)
        {
            return prefix + x;
        }
    }
}