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
    /// BasicEvent
    /// </summary>
    [DataContract]
    public partial class BasicEvent : ModelEvent, IEquatable<BasicEvent>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicEvent" /> class.
        /// </summary>
        /// <param name="observed">observed (required).</param>
        public BasicEvent(Reference observed = default(Reference)) : base()
        {
            // to ensure "observed" is required (not null)
            if (observed == null)
            {
                throw new InvalidDataException("observed is a required property for BasicEvent and cannot be null");
            }
            else
            {
                this.Observed = observed;
            }
        }

        /// <summary>
        /// Gets or Sets Observed
        /// </summary>
        [DataMember(Name = "observed", EmitDefaultValue = false)]
        public Reference Observed { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class BasicEvent {\n");
            sb.Append("  ").Append(base.ToString().Replace("\n", "\n  ")).Append("\n");
            sb.Append("  Observed: ").Append(Observed).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public override string ToJson()
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
            return this.Equals(input as BasicEvent);
        }

        /// <summary>
        /// Returns true if BasicEvent instances are equal
        /// </summary>
        /// <param name="input">Instance of BasicEvent to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(BasicEvent input)
        {
            if (input == null)
                return false;

            return base.Equals(input) &&
                (
                    this.Observed == input.Observed ||
                    (this.Observed != null &&
                    this.Observed.Equals(input.Observed))
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
                int hashCode = base.GetHashCode();
                if (this.Observed != null)
                    hashCode = hashCode * 59 + this.Observed.GetHashCode();
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
