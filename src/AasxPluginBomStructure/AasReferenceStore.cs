/*
Copyright (c) 2018-2019 Festo AG & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminShellNS;

namespace AasxPluginBomStructure
{
    public class AasReferenceStore
    {
        private class MultiValueDictionary<K, V>
        {
            private Dictionary<K, List<V>> dict = new Dictionary<K, List<V>>();
            public void Add(K key, V value)
            {
                if (dict.TryGetValue(key, out var list))
                    list.Add(value);
                else
                    dict.Add(key, new List<V> { value });
            }

            public bool ContainsKey(K key) => dict.ContainsKey(key);

            public List<V> this[K key] => dict[key];
        }

        private MultiValueDictionary<uint, AdminShell.Referable> dict =
            new MultiValueDictionary<uint, AdminShellV20.Referable>();

        private static System.Security.Cryptography.SHA256 HashProvider = System.Security.Cryptography.SHA256.Create();

        private uint ComputeHashOnReference(AdminShell.Reference r)
        {
            // access
            if (r == null || r.Keys == null)
                return 0;

            // use memory stream for effcient behaviour
            byte[] dataBytes = null;
            using (var mems = new MemoryStream())
            {
                foreach (var k in r.Keys)
                {
                    var bs = System.Text.Encoding.UTF8.GetBytes(k.type.Trim().ToLower());
                    mems.Write(bs, 0, bs.Length);

                    bs = System.Text.Encoding.UTF8.GetBytes(k.idType.Trim().ToLower());
                    mems.Write(bs, 0, bs.Length);

                    bs = System.Text.Encoding.UTF8.GetBytes(k.value.Trim().ToLower());
                    mems.Write(bs, 0, bs.Length);
                }

                dataBytes = mems.ToArray();
            }

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // ReSharper disable HeuristicUnreachableCode
            if (dataBytes == null)
                return 0;
            // ReSharper enable ConditionIsAlwaysTrueOrFalse
            // ReSharper enable HeuristicUnreachableCode

            uint sum = 0;
            foreach (var b in dataBytes)
                sum += b;
            return sum;
        }

        private void RecurseIndexSME(AdminShell.Reference currRef, AdminShell.SubmodelElement sme)
        {
            // access
            if (currRef == null || sme == null)
                return;

            // add to the currRef
            currRef.Keys.Add(
                new AdminShell.Key(
                    sme.GetElementName(), false, AdminShell.Identification.IdShort, sme.idShort));

            // index
            var hk = ComputeHashOnReference(currRef);
            dict.Add(hk, sme);

            // recurse
            var childs = (sme as AdminShell.IEnumerateChildren)?.EnumerateChildren();
            if (childs != null)
                foreach (var sme2 in childs)
                    RecurseIndexSME(currRef, sme2?.submodelElement);

            // remove from currRef
            currRef.Keys.RemoveAt(currRef.Keys.Count - 1);
        }

        public void Index(AdminShell.Submodel sm)
        {
            // access
            if (sm == null)
                return;

            // make curr ref and index
            var currRef = sm.GetReference();
            dict.Add(ComputeHashOnReference(currRef), sm);

            // recurse
            foreach (var sme in sm.EnumerateChildren())
                RecurseIndexSME(currRef, sme?.submodelElement);
        }

        public void Index(AdminShell.AdministrationShellEnv env)
        {
            // access
            if (env == null || env.Submodels == null)
                return;

            // iterate
            foreach (var sm in env.Submodels)
                this.Index(sm);
        }

        public AdminShell.Referable FindReferableByReference(AdminShell.Reference r)
        {
            var hk = ComputeHashOnReference(r);
            if (hk == 0 || !dict.ContainsKey(hk))
                return null;

            foreach (var test in dict[hk])
            {
                var xx = (test as AdminShell.IGetReference)?.GetReference();
                if (xx != null && xx.Matches(r))
                    return test;
            }

            return null;
        }

    }
}
