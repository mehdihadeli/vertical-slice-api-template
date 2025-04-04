// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace Vertical.Slice.Template.ContractTests.KiotaApiClient.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class AddressDto : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The city property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? City { get; set; }
#nullable restore
#else
        public string City { get; set; }
#endif
        /// <summary>The fullAddress property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? FullAddress { get; set; }
#nullable restore
#else
        public string FullAddress { get; set; }
#endif
        /// <summary>The postalCode property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? PostalCode { get; set; }
#nullable restore
#else
        public string PostalCode { get; set; }
#endif
        /// <summary>The state property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? State { get; set; }
#nullable restore
#else
        public string State { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.AddressDto"/> and sets the default values.
        /// </summary>
        public AddressDto()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.AddressDto"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.AddressDto CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::Vertical.Slice.Template.ContractTests.KiotaApiClient.Models.AddressDto();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "city", n => { City = n.GetStringValue(); } },
                { "fullAddress", n => { FullAddress = n.GetStringValue(); } },
                { "postalCode", n => { PostalCode = n.GetStringValue(); } },
                { "state", n => { State = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("city", City);
            writer.WriteStringValue("fullAddress", FullAddress);
            writer.WriteStringValue("postalCode", PostalCode);
            writer.WriteStringValue("state", State);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
