﻿/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xceed.Wpf.AvalonDock.Controls
{
    class FullWeakDictionary<K,V> where K : class
    {
        public FullWeakDictionary()
        {}

        List<WeakReference> _keys = new List<WeakReference>();
        List<WeakReference> _values = new List<WeakReference>();

        public V this[K key]
        {
            get
            {
                V valueToReturn;
                if (!GetValue(key, out valueToReturn))
                    throw new ArgumentException();
                return valueToReturn;
            }
            set
            {
                SetValue(key, value);
            }
        }

        public bool ContainsKey(K key)
        {
            CollectGarbage();
            return -1 != _keys.FindIndex(k => k.GetValueOrDefault<K>() == key);
        }

        public void SetValue(K key, V value)
        {
            CollectGarbage();
            int vIndex = _keys.FindIndex(k => k.GetValueOrDefault<K>() == key);
            if (vIndex > -1)
                _values[vIndex] = new WeakReference(value);
            else
            {
                _values.Add(new WeakReference(value));
                _keys.Add(new WeakReference(key));
            }            
        }

        public bool GetValue(K key, out V value)
        {
            CollectGarbage();
            int vIndex = _keys.FindIndex(k => k.GetValueOrDefault<K>() == key);
            value = default(V);
            if (vIndex == -1)
                return false;
            value = _values[vIndex].GetValueOrDefault<V>();
            return true;
        }


        void CollectGarbage()
        { 
            int vIndex = 0; 

            do
            { 
                vIndex = _keys.FindIndex(vIndex, k => !k.IsAlive);
                if (vIndex >= 0)
                {
                    _keys.RemoveAt(vIndex);
                    _values.RemoveAt(vIndex);
                }
            }
            while (vIndex >= 0);

            vIndex = 0; 
            do
            {
                vIndex = _values.FindIndex(vIndex, v => !v.IsAlive);
                if (vIndex >= 0)
                {
                    _values.RemoveAt(vIndex);
                    _keys.RemoveAt(vIndex);
                }
            }
            while (vIndex >= 0);
        }
    }
}
