/********************************************************************************
* Copyright (c) {2024} Contributors to the Eclipse Foundation
*
* See the NOTICE file(s) distributed with this work for additional
* information regarding copyright ownership.
*
* This program and the accompanying materials are made available under the
* terms of the Apache License Version 2.0 which is available at
* https://www.apache.org/licenses/LICENSE-2.0
*
* SPDX-License-Identifier: Apache-2.0
********************************************************************************/

/* 
 * DotAAS Part 2 | HTTP/REST | Entire Interface Collection
 *
 * The entire interface collection as part of Details of the Asset Administration Shell Part 2
 *
 * OpenAPI spec version: Final-Draft
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwaggerDateConverter = IO.Swagger.Client.SwaggerDateConverter;

namespace IO.Swagger.Model
{
    /// <summary>
    /// AssetAdministrationShellEnvironment
    /// </summary>
    [DataContract]
    public partial class AssetAdministrationShellEnvironment : IEquatable<AssetAdministrationShellEnvironment>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetAdministrationShellEnvironment" /> class.
        /// </summary>
        /// <param name="assetAdministrationShells">assetAdministrationShells.</param>
        /// <param name="conceptDescriptions">conceptDescriptions.</param>
        /// <param name="submodels">submodels.</param>
        public AssetAdministrationShellEnvironment(List<AssetAdministrationShell> assetAdministrationShells = default(List<AssetAdministrationShell>), List<ConceptDescription> conceptDescriptions = default(List<ConceptDescription>), List<Submodel> submodels = default(List<Submodel>))
        {
            this.AssetAdministrationShells = assetAdministrationShells;
            this.ConceptDescriptions = conceptDescriptions;
            this.Submodels = submodels;
        }

        /// <summary>
        /// Gets or Sets AssetAdministrationShells
        /// </summary>
        [DataMember(Name = "assetAdministrationShells", EmitDefaultValue = false)]
        public List<AssetAdministrationShell> AssetAdministrationShells { get; set; }

        /// <summary>
        /// Gets or Sets ConceptDescriptions
        /// </summary>
        [DataMember(Name = "conceptDescriptions", EmitDefaultValue = false)]
        public List<ConceptDescription> ConceptDescriptions { get; set; }

        /// <summary>
        /// Gets or Sets Submodels
        /// </summary>
        [DataMember(Name = "submodels", EmitDefaultValue = false)]
        public List<Submodel> Submodels { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AssetAdministrationShellEnvironment {\n");
            sb.Append("  AssetAdministrationShells: ").Append(AssetAdministrationShells).Append("\n");
            sb.Append("  ConceptDescriptions: ").Append(ConceptDescriptions).Append("\n");
            sb.Append("  Submodels: ").Append(Submodels).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as AssetAdministrationShellEnvironment);
        }

        /// <summary>
        /// Returns true if AssetAdministrationShellEnvironment instances are equal
        /// </summary>
        /// <param name="input">Instance of AssetAdministrationShellEnvironment to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AssetAdministrationShellEnvironment input)
        {
            if (input == null)
                return false;

            return
                (
                    this.AssetAdministrationShells == input.AssetAdministrationShells ||
                    this.AssetAdministrationShells != null &&
                    input.AssetAdministrationShells != null &&
                    this.AssetAdministrationShells.SequenceEqual(input.AssetAdministrationShells)
                ) &&
                (
                    this.ConceptDescriptions == input.ConceptDescriptions ||
                    this.ConceptDescriptions != null &&
                    input.ConceptDescriptions != null &&
                    this.ConceptDescriptions.SequenceEqual(input.ConceptDescriptions)
                ) &&
                (
                    this.Submodels == input.Submodels ||
                    this.Submodels != null &&
                    input.Submodels != null &&
                    this.Submodels.SequenceEqual(input.Submodels)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.AssetAdministrationShells != null)
                    hashCode = hashCode * 59 + this.AssetAdministrationShells.GetHashCode();
                if (this.ConceptDescriptions != null)
                    hashCode = hashCode * 59 + this.ConceptDescriptions.GetHashCode();
                if (this.Submodels != null)
                    hashCode = hashCode * 59 + this.Submodels.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
