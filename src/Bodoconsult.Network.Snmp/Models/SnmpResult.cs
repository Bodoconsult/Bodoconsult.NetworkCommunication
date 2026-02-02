namespace Bodoconsult.Inventory.Model
{
    public class SnmpResult
    {
        /// <summary>
        /// Oid found
        /// </summary>
        public string Oid { get; set; }

        /// <summary>
        /// Value found for the OID found
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// Clear text description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Fully qualified name of the OID
        /// </summary>
        public string FullName { get; set; }

    }
}