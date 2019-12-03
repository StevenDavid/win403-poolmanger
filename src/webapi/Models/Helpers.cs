using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

namespace webapi.Models
{
  public class Helpers
  {

    public static string currentMSSQL_CS = "";
    public static string currentAurora_CS = "";
    public static string currentAuroraSL_CS = "";
    public static string GetMSSQLConnectionString()
    {
      Console.WriteLine("GetMSSQLConnectionString Called");
      if (currentMSSQL_CS == "") {
        DB_CS currentCSObject = new DB_CS();
        currentCSObject.host = "{server address}";
        currentCSObject.username = "{user name}";
        currentCSObject.password = "{password}";
        currentMSSQL_CS = $"Data Source={currentCSObject.host};User ID={currentCSObject.username};Password={currentCSObject.password}";
      }
      return currentMSSQL_CS;
    }

    public static string GetMSSQLConnectionFromSM()
    {
      Console.WriteLine("GetMSSQLConnectionString Called");
      if (currentMSSQL_CS == "") {
        string rawCS = GetSecret("testdb1");
        DB_CS currentCSObject = JsonConvert.DeserializeObject<DB_CS>(rawCS);
        currentMSSQL_CS = $"Data Source={currentCSObject.host};User ID={currentCSObject.username};Password={currentCSObject.password}";
      }
      return currentMSSQL_CS;
    }

    public static string GetSecret(string secretName)
    {
        
        Console.WriteLine("GetSecret Called");
        string region = "us-east-1";
        string secret = "";

        MemoryStream memoryStream = new MemoryStream();

        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

        GetSecretValueRequest request = new GetSecretValueRequest();
        request.SecretId = secretName;
        request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

        GetSecretValueResponse response = null;

        // In this sample we only handle the specific exceptions for the 'GetSecretValue' API.
        // See https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
        // We rethrow the exception by default.

        try
        {
            response = client.GetSecretValueAsync(request).Result;
            Console.WriteLine("Got secret");
        }
        catch (DecryptionFailureException e)
        {
            // Secrets Manager can't decrypt the protected secret text using the provided KMS key.
            // Deal with the exception here, and/or rethrow at your discretion.
            Console.WriteLine("DecryptionFailureException, message=" + e.Message);
            throw;
        }
        catch (InternalServiceErrorException e)
        {
            // An error occurred on the server side.
            // Deal with the exception here, and/or rethrow at your discretion.
            Console.WriteLine("InternalServiceErrorException, message=" + e.Message);
            throw;
        }
        catch (InvalidParameterException e)
        {
            // You provided an invalid value for a parameter.
            // Deal with the exception here, and/or rethrow at your discretion
            Console.WriteLine("InvalidParameterException, message=" + e.Message);
            throw;
        }
        catch (InvalidRequestException e)
        {
            // You provided a parameter value that is not valid for the current state of the resource.
            // Deal with the exception here, and/or rethrow at your discretion.
            Console.WriteLine("InvalidRequestException, message=" + e.Message);
            throw;
        }
        catch (ResourceNotFoundException e)
        {
            // We can't find the resource that you asked for.
            // Deal with the exception here, and/or rethrow at your discretion.
            Console.WriteLine("ResourceNotFoundException, message=" + e.Message);
            throw;
        }
        catch (System.AggregateException ae)
        {
            // More than one of the above exceptions were triggered.
            // Deal with the exception here, and/or rethrow at your discretion.
            Console.WriteLine("AggregateException, message=" + ae.Message);
            Console.WriteLine("AggregateException, InnerExceptionsCount=" + ae.InnerExceptions.Count);
            Console.WriteLine("AggregateException, InnerExceptions=" + ae.InnerExceptions.ToString());
            throw;
        }

        // Decrypts secret using the associated KMS CMK.
        // Depending on whether the secret is a string or binary, one of these fields will be populated.
        if (response.SecretString != null)
        {
            secret = response.SecretString;
        }
        else
        {
            memoryStream = response.SecretBinary;
            StreamReader reader = new StreamReader(memoryStream);
            string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
        }

        return secret;
    }

  }

}