﻿/*
Copyright (c) 2018-2021 Festo AG & Co. KG <https://www.festo.com/net/de_de/Forms/web/contact_international>
Author: Michael Hoffmeister

This source code is licensed under the Apache License 2.0 (see LICENSE.txt).

This source code may use other Open Source software components (see LICENSE.txt).
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AasCore.Aas3_0_RC02;
using AdminShellNS;
using Extensions;
using Newtonsoft.Json;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnassignedField.Global

namespace AasxPluginBomStructure
{
    public enum BomLinkDirection { None, Forward, Backward, Both }

    public class BomLinkStyle // : IReferable
    {
        public Key Match;
        public bool Skip;
        public BomLinkDirection Direction;
        public string Color;
        public double Width;
        public string Text;
        public double FontSize;
        public bool Dashed, Bold, Dotted;

        // public Reference GetReference(bool includeParents = true) => new Reference(Match);
    }

    public class BomLinkStyleList : List<BomLinkStyle>
    {
        [JsonIgnore]
        public AasReferenceStore<BomLinkStyle> Store = new AasReferenceStore<BomLinkStyle>();

        public void Index()
        {
            foreach (var ls in this)
                Store.Index(ExtendReference.CreateFromKey(ls.Match), ls);
        }
    }

    public class BomNodeStyle // : IGetReference
    {
        public Key Match;
        public bool Skip;

        public string Shape;
        public string Background, Foreground;
        public double LineWidth;
        public double Radius;
        public string Text;
        public double FontSize;
        public bool Dashed, Bold, Dotted;

        // public Reference GetReference(bool includeParents = true) => new Reference(Match);
    }

    public class BomNodeStyleList : List<BomNodeStyle>
    {
        [JsonIgnore]
        public AasReferenceStore<BomNodeStyle> Store = new AasReferenceStore<BomNodeStyle>();

        public void Index()
        {
            foreach (var ls in this)
                Store.Index(ExtendReference.CreateFromKey(ls.Match), ls);
        }
    }

    public class BomStructureOptionsRecord
    {
        public List<Key> AllowSubmodelSemanticId = new List<Key>();

        public int Layout;
        public bool? Compact;

        public BomLinkStyleList LinkStyles = new BomLinkStyleList();
        public BomNodeStyleList NodeStyles = new BomNodeStyleList();

        public void Index()
        {
            LinkStyles.Index();
            NodeStyles.Index();
        }

        public BomLinkStyle FindFirstLinkStyle(Reference semId)
        {
            if (semId == null)
                return null;
            return LinkStyles.Store.FindElementByReference(semId, MatchMode.Relaxed);
        }

        public BomNodeStyle FindFirstNodeStyle(Reference semId)
        {
            if (semId == null)
                return null;
            return NodeStyles.Store.FindElementByReference(semId, MatchMode.Relaxed);
        }

    }

    public class BomStructureOptionsRecordList : List<BomStructureOptionsRecord>
    {
        public BomStructureOptionsRecordList() { }

        public BomStructureOptionsRecordList(IEnumerable<BomStructureOptionsRecord> collection) : base(collection) { }

        public BomLinkStyle FindFirstLinkStyle(Reference semId)
        {
            foreach (var rec in this)
            {
                var res = rec.FindFirstLinkStyle(semId);
                if (res != null)
                    return res;
            }
            return null;
        }

        public BomNodeStyle FindFirstNodeStyle(Reference semId)
        {
            foreach (var rec in this)
            {
                var res = rec.FindFirstNodeStyle(semId);
                if (res != null)
                    return res;
            }
            return null;
        }
    }

    public class BomStructureOptions : AasxIntegrationBase.AasxPluginOptionsBase
    {
        public List<BomStructureOptionsRecord> Records = new List<BomStructureOptionsRecord>();

        /// <summary>
        /// Create a set of minimal options
        /// </summary>
        public static BomStructureOptions CreateDefault()
        {
            var rec = new BomStructureOptionsRecord();
            rec.AllowSubmodelSemanticId.Add(
                new Key(KeyTypes.Submodel, "http://smart.festo.com/id/type/submodel/BOM/1/1"));

            var opt = new BomStructureOptions();
            opt.Records.Add(rec);

            return opt;
        }

        /// <summary>
        /// For faster access of styles, index them by a hash map
        /// </summary>
        public void Index()
        {
            foreach (var rec in Records)
                rec.Index();
        }

        /// <summary>
        /// Find matching options records
        /// </summary>
        public IEnumerable<BomStructureOptionsRecord> MatchingRecords(Reference semId)
        {
            foreach (var rec in Records)
                if (rec.AllowSubmodelSemanticId != null)
                    foreach (var x in rec.AllowSubmodelSemanticId)
                        if (semId != null && semId.MatchesExactlyOneKey(x))
                            yield return rec;
        }
    }
}
