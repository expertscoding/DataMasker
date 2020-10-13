namespace DataMasker
{
    /// <summary>
    /// 
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The api data type, supports {{entity.property}} e.g. {{address.FullAddress}}
        /// </summary>
        Bogus,

        /// <summary>
        /// The first name
        /// </summary>
        FirstName,

        /// <summary>
        /// The last name
        /// </summary>
        LastName,

        /// <summary>
        /// The date of birth
        /// </summary>
        DateOfBirth,

        /// <summary>
        /// The rant
        /// </summary>
        Rant,

        /// <summary>
        /// The lorem
        /// </summary>
        Lorem,

        /// <summary>
        /// The string format
        /// </summary>
        StringFormat,

        /// <summary>
        /// The full address
        /// </summary>
        FullAddress,

        /// <summary>
        /// The phone number
        /// </summary>
        PhoneNumber,

        /// <summary>
        /// Indicates that the column value is computed from other indicated columns
        /// </summary>
        Computed,
        
        /// <summary>
        /// A random decimal number from min to max
        /// </summary>
        Decimal,
        
        /// <summary>
        /// Spanish Tax Identification Number (Número de Identificación Fiscal). For people
        /// </summary>
        Nif,
        
        /// <summary>
        /// Spanish Tax Identification Code (Código de Identificación Fiscal). For companies 
        /// </summary>
        Cif,
        
        /// <summary>
        /// Spanish Social Security Affiliation Number (Número de Afiliación a la Seguridad Social). For people
        /// </summary>
        Naf
    }
}
