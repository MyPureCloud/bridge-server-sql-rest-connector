using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.IO;
using inin.Bridge.WebServices.Datadip.Lib;
using System.Data.SqlClient;
using System.Reflection;
using System.Diagnostics;

namespace PureCloudRESTService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WebServicesImplementation : IWebServicesServer
    {
        internal Config config = new Config();
        internal string URL
        {
            get { return config.URL; }
        }
        private string configFile
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\ServiceConfig.json";
            }
        }

        public WebServicesImplementation()
        {
            config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(readFromFile(configFile));
        }

        public ResponseContact GetContactByPhoneNumber(PhoneNumberRequest req)
        {
            ResponseContact rc = new ResponseContact();            
            rc.Contact = SQLGetContact(config.getContactByPhoneNumber.Replace("%1", req.PhoneNumber));
            return rc;
        }

        public ResponseAccount GetAccountByPhoneNumber(PhoneNumberRequest req)
        {
            ResponseAccount retVal = new ResponseAccount();
            retVal.Account = SQLGetAccount(config.getAccountByPhoneNumber.Replace("%1", req.PhoneNumber));
            return retVal;
        }

        public ResponseAccount GetAccountByAccountNumber(AccountNumberRequest req)
        {
            ResponseAccount retVal = new ResponseAccount();            
            retVal.Account = SQLGetAccount(config.getAccountByAccountNumber.Replace("%1", req.AccountNumber)); ;
            return retVal;
        }

        public ResponseAccount GetAccountByContactId(ContactIdRequest cidr)
        {
            ResponseAccount retVal = new ResponseAccount();
            retVal.Account = SQLGetAccount(config.getAccountByAccountNumber.Replace("%1", cidr.ContactId)); ;
            return retVal;
        }

        public ResponseCase GetMostRecentOpenCaseByContactId(ContactIdRequest cidr)
        {

            throw new WebFaultException(HttpStatusCode.NotImplemented);
        }

        private Contact SQLGetContact(string query)
        {
            using (SqlConnection connection = new SqlConnection(config.connectionString))
            {
                Contact contact = new Contact();
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        // Loop through a list of contact fields that are configured and add them to the return object
                        foreach (string field in config.contactConfig.fields)
                        {
                            try
                            {
                                contact.GetType().GetProperty(field).SetValue(contact, reader[field].ToString(), null);
                            }
                            catch (Exception ex)
                            {
                                throw new WebFaultException<string>(String.Format("Column <{0}> not found in query results: {1}", field, ex.Message), HttpStatusCode.InternalServerError);
                            }
                        }
                        // Get all the phone numbers
                        contact.PhoneNumbers = getPhoneNumbers(reader, config.contactConfig.phoneCount, config.contactConfig.phoneMappings);
                        // Get all the email addresses
                        contact.EmailAddresses = getEmails(reader, config.contactConfig.emailCount, config.contactConfig.emailMappings);
                        if (config.contactConfig.getAddress)
                        {
                            // Get the address
                            contact.Address = getAddress(reader, config.contactConfig.addressFields);
                        }
                    }
                    else
                    {
                        throw new WebFaultException<string>("No results found", HttpStatusCode.NoContent);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    reader.Close();
                }
                return contact;
            }
        }

        private Account SQLGetAccount(string query)
        {
            using (SqlConnection connection = new SqlConnection(config.connectionString))
            {
                Account account = new Account();
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    if (reader.Read())
                    {
                        // Loop through a list of contact fields that are configured and add them to the return object
                        foreach (string field in config.accountConfig.fields)
                        {
                            try
                            {
                                account.GetType().GetProperty(field).SetValue(account, reader[field], null);
                            }
                            catch (Exception ex)
                            {
                                throw new WebFaultException<string>(String.Format("Column <{0}> not found in query results: {1}", field, ex.Message), HttpStatusCode.InternalServerError);
                            }
                        }
                        // Get all the phone numbers
                        account.PhoneNumbers = getPhoneNumbers(reader, config.accountConfig.phoneCount, config.accountConfig.phoneMappings);
                        // Get all the email addresses
                        account.EmailAddresses = getEmails(reader, config.accountConfig.emailCount, config.accountConfig.emailMappings);
                        // Get the address
                        if (config.accountConfig.getAddress)
                        {
                            account.Addresses.Address.Add(getAddress(reader, config.accountConfig.addressFields));
                        }
                    }
                    else
                    {
                        throw new WebFaultException<string>("No results found", HttpStatusCode.NoContent);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    reader.Close();
                }
                return account;
            }
        }

        private PhoneNumbers getPhoneNumbers(SqlDataReader reader, int count, List<string> mappings)
        {
            PhoneNumbers numbers = new PhoneNumbers();
            for (int i = 0; i < count; i++)
            {
                try {
                    PhoneNumber phone = new PhoneNumber();
                    phone.Number = reader[mappings[i]].ToString();
                    phone.PhoneType = i + 1;
                    numbers.PhoneNumber.Add(phone);
                }
                catch(Exception ex)
                {
                    throw new WebFaultException<string>(String.Format("Column <{0}> not found in query results: {1}", mappings[i], ex.Message), HttpStatusCode.InternalServerError);
                }
            }
            return numbers;
        }

        private EmailAddresses getEmails(SqlDataReader reader, int count, List<string> mappings)
        {
            EmailAddresses addresses = new EmailAddresses();
            for (int i = 0; i < count; i++)
            {
                try {
                    EmailAddressModel email = new EmailAddressModel();
                    email.EmailAddress = reader[mappings[i]].ToString();
                    email.EmailType = i + 1;
                    addresses.EmailAddress.Add(email);
                }
                catch (Exception ex)
                {
                    throw new WebFaultException<string>(string.Format("Column <{0}> not found in query results: {1}", mappings[i], ex.Message), HttpStatusCode.InternalServerError);
                }
            }
            return addresses;
        }

        private Address getAddress(SqlDataReader reader, List<string> fields)
        {
            Address address = new Address();
            
            foreach (string field in fields)
            {
                try {
                    address.GetType().GetProperty(field).SetValue(address, reader[field], null);
                }
                catch(Exception ex)
                {
                    throw new WebFaultException<string>(string.Format("Column <{0}> not found in query results: {1}", field, ex.Message), HttpStatusCode.InternalServerError);
                }            
            }
            return address;
        }

        private string readFromFile(string filePath)
        {
            try
            {
                StreamReader sr = new StreamReader(filePath);
                string returnVal = sr.ReadToEnd();
                return returnVal;
            }
            catch(Exception ex)
            {
                EventLog.WriteEntry("PureCloud REST Service", string.Format("Failed to open configuration file <{0}>.  Stopping service.", filePath));
                throw ex;
            }
        }
    }
}
