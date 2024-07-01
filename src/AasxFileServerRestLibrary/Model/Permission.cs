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
    /// Permission
    /// </summary>
    [DataContract]
    public partial class Permission : IEquatable<Permission>, IValidatableObject
    {
        /// <summary>
        /// Defines KindOfPermission
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum KindOfPermissionEnum
        {
            /// <summary>
            /// Enum Allow for value: Allow
            /// </summary>
            [EnumMember(Value = "Allow")]
            Allow = 1,
            /// <summary>
            /// Enum Deny for value: Deny
            /// </summary>
            [EnumMember(Value = "Deny")]
            Deny = 2,
            /// <summary>
            /// Enum NotApplicable for value: NotApplicable
            /// </summary>
            [EnumMember(Value = "NotApplicable")]
            NotApplicable = 3,
            /// <summary>
            /// Enum Undefined for value: Undefined
            /// </summary>
            [EnumMember(Value = "Undefined")]
            Undefined = 4
        }
        /// <summary>
        /// Gets or Sets KindOfPermission
        /// </summary>
        [DataMember(Name = "kindOfPermission", EmitDefaultValue = false)]
        public KindOfPermissionEnum KindOfPermission { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Permission" /> class.
        /// </summary>
        /// <param name="kindOfPermission">kindOfPermission (required).</param>
        /// <param name="permission">permission (required).</param>
        public Permission(KindOfPermissionEnum kindOfPermission = default(KindOfPermissionEnum), Reference permission = default(Reference))
        {
            // to ensure "kindOfPermission" is required (not null)
            if (kindOfPermission == null)
            {
                throw new InvalidDataException("kindOfPermission is a required property for Permission and cannot be null");
            }
            else
            {
                this.KindOfPermission = kindOfPermission;
            }
            // to ensure "permission" is required (not null)
            if (permission == null)
            {
                throw new InvalidDataException("permission is a required property for Permission and cannot be null");
            }
            else
            {
                this._Permission = permission;
            }
        }


        /// <summary>
        /// Gets or Sets _Permission
        /// </summary>
        [DataMember(Name = "permission", EmitDefaultValue = false)]
        public Reference _Permission { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Permission {\n");
            sb.Append("  KindOfPermission: ").Append(KindOfPermission).Append("\n");
            sb.Append("  _Permission: ").Append(_Permission).Append("\n");
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
            return this.Equals(input as Permission);
        }

        /// <summary>
        /// Returns true if Permission instances are equal
        /// </summary>
        /// <param name="input">Instance of Permission to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Permission input)
        {
            if (input == null)
                return false;

            return
                (
                    this.KindOfPermission == input.KindOfPermission ||
                    (this.KindOfPermission != null &&
                    this.KindOfPermission.Equals(input.KindOfPermission))
                ) &&
                (
                    this._Permission == input._Permission ||
                    (this._Permission != null &&
                    this._Permission.Equals(input._Permission))
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
                if (this.KindOfPermission != null)
                    hashCode = hashCode * 59 + this.KindOfPermission.GetHashCode();
                if (this._Permission != null)
                    hashCode = hashCode * 59 + this._Permission.GetHashCode();
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
