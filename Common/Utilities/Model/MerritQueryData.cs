using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace Microsoft.Research.DataOnboarding.Utilities.Model
{
    public class MerritQueryData
    {
        #region Private variables
        /// <summary>
        /// keyValuePair private object
        /// </summary>
        private DKeyValuePair<string, string>[] keyValuePair;

        /// <summary>
        /// parameterMetadata private object
        /// </summary>
        private ParameterMetadataDetail[] parameterMetadata;

        /// <summary>
        /// MetaDataDetail private object
        /// </summary>
        private MetaDataDetail metaDataDetail;
        #endregion

        /// <summary>
        /// Default QueryData Constructor
        /// Initializes a new instance of the <see cref="QueryData" /> class.
        /// </summary>
        public MerritQueryData()
        {
            this.metaDataDetail = new MetaDataDetail();
        }

        /// <summary>
        /// Gets or sets MetaDataDetail
        /// </summary>
        public MetaDataDetail MetaDataDetail
        {
            get { return this.metaDataDetail; }
            set { this.metaDataDetail = value; }
        }

        public string MetadataXML
        {
            get;
            set;
        }

        /// <summary>
        /// DKeyValuePair Constructor with following parameter
        /// </summary>
        /// <param name="key">key parameter</param>
        /// <returns>returns collection of DKValuePair values</returns>
        public DKeyValuePair<string, string> this[string key]
        {
            get
            {
                DKeyValuePair<string, string> temp = null;
                if (this.KeyValuePair.Length != 0 && this.KeyValuePair.Length == 1)
                {
                    return this.KeyValuePair[0];
                }
                else if (this.KeyValuePair.Length > 0)
                {
                    for (int index = 0; index <= this.KeyValuePair.Length - 1; index++)
                    {
                        temp = this.KeyValuePair[index];
                        if (temp.Key == key)
                        {
                            return temp;
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets parameterMetadataDetail 
        /// </summary>
        public ParameterMetadataDetail[] ParameterMetadata
        {
            get
            {
                if (this.parameterMetadata == null)
                {
                    this.parameterMetadata = new ParameterMetadataDetail[0];
                }

                return this.parameterMetadata;
            }

            set
            {
                this.parameterMetadata = value;
            }
        }

        /// <summary>
        /// Gets or sets KeyValuePair 
        /// </summary>
        public DKeyValuePair<string, string>[] KeyValuePair
        {
            get
            {
                if (this.keyValuePair == null)
                {
                    this.keyValuePair = new DKeyValuePair<string, string>[0];
                }

                return this.keyValuePair;
            }

            set
            {
                this.keyValuePair = value;
            }
        }
    }

    public class DKeyValuePair<TKey, TValue>
    {
        /// <summary>
        /// private TKey field
        /// </summary>
        private TKey key;

        /// <summary>
        /// private TValue field
        /// </summary>
        private TValue value;

        public DKeyValuePair()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DKeyValuePair<TKey, TValue>"/> class.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <param name="value">Value value.</param>
        public DKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Gets or sets TKey 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Justification = "Created this method and variables as generic way to use")]
        public TKey Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// Gets or sets TValue 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value", Justification = "Created this method and variables as generic way to use")]
        public TValue Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }

    public class ParameterMetadataDetail
    {
        public ParameterMetadataDetail()
        {
            this.AttributeList = new List<ParameterAttribute>();
        }

        /// <summary>
        /// Gets or sets the Entity Name
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Gets or sets the Entity Description
        /// </summary>
        public string EntityDescription { get; set; }

        /// <summary>
        /// Gets the Attribute List
        /// </summary>
        public IList<ParameterAttribute> AttributeList { get; set; }
    }

    public class ParameterAttribute
    {
        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets  Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets  Type
        /// </summary>
        public string ParamType { get; set; }

        /// <summary>
        /// Gets or sets Units
        /// </summary>
        public string Units { get; set; }
    }

    public class MetaDataDetail
    {
        /// <summary>
        /// Gets or sets Meta Data Id
        /// </summary>
        public int MetadataID { get; set; }

        /// <summary>
        /// Gets or sets Meta Data Name
        /// </summary>
        public string MetadataName { get; set; }

        /// <summary>
        /// Gets or sets  Meta Data Type Id
        /// </summary>
        public int MetadataTypeID { get; set; }

        /// <summary>
        /// Gets or sets Meta Data Mapping XML
        /// </summary>
        public string MetadataMappingXML { get; set; }

        /// <summary>
        /// Gets or sets Mapping XML
        /// </summary>
        public string MetadataXML { get; set; }
    }
}

