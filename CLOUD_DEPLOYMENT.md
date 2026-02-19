# Cloud Deployment Configuration Guide

## Overview
This application has been configured for cloud-native deployment on AWS. All sensitive configuration values have been externalized to environment variables.

## Required Environment Variables

### Database Configuration
- **DATABASE_CONNECTION_STRING**: SQL Server connection string for the application database
  - Example: `Server=myserver.rds.amazonaws.com;Database=SampleDotNet6Db;User Id=dbuser;Password=dbpassword;Encrypt=true;TrustServerCertificate=false;`
  - **Required**: Yes (for production)
  - **AWS Service**: Store in AWS Secrets Manager or AWS Systems Manager Parameter Store
  - **Note**: If not provided, the application will fall back to in-memory database (not suitable for production)

### JWT Authentication Configuration
- **JWT_SECRET_KEY**: Secret key for JWT token signing (minimum 32 characters)
  - Example: `your-super-secret-jwt-key-that-is-at-least-256-bits-long`
  - **Required**: Yes
  - **AWS Service**: Store in AWS Secrets Manager
  - **Security**: Must be at least 32 characters long. Application will throw an error if not set properly.

- **JWT_ISSUER**: JWT token issuer identifier
  - Example: `SampleDotNet6App`
  - **Required**: No (defaults to "SampleDotNet6App")
  - **AWS Service**: Can be set as environment variable in ECS/EKS/Elastic Beanstalk

- **JWT_AUDIENCE**: JWT token audience identifier
  - Example: `SampleDotNet6App`
  - **Required**: No (defaults to "SampleDotNet6App")
  - **AWS Service**: Can be set as environment variable in ECS/EKS/Elastic Beanstalk

### CORS Configuration
- **CORS_ALLOWED_ORIGINS**: Comma-separated list of allowed origins for CORS
  - Example: `https://myapp.example.com,https://admin.example.com`
  - **Required**: No (defaults to "*" which allows all origins - not recommended for production)
  - **AWS Service**: Can be set as environment variable in ECS/EKS/Elastic Beanstalk
  - **Note**: For production, specify exact origins. Use "*" only for development.

## AWS Deployment Options

### Option 1: AWS Elastic Beanstalk
1. Create an Elastic Beanstalk environment for .NET
2. Configure environment variables in the Elastic Beanstalk console
3. For sensitive values (JWT_SECRET_KEY, DATABASE_CONNECTION_STRING), reference AWS Secrets Manager:
   ```
   JWT_SECRET_KEY={{resolve:secretsmanager:MyAppSecrets:SecretString:JwtKey}}
   DATABASE_CONNECTION_STRING={{resolve:secretsmanager:MyAppSecrets:SecretString:DbConnectionString}}
   ```

### Option 2: AWS ECS (Elastic Container Service)
1. Build Docker image from the application
2. Create ECS task definition with environment variables
3. Use secrets in task definition to reference AWS Secrets Manager:
   ```json
   "secrets": [
     {
       "name": "JWT_SECRET_KEY",
       "valueFrom": "arn:aws:secretsmanager:region:account-id:secret:MyAppSecrets:JwtKey::"
     },
     {
       "name": "DATABASE_CONNECTION_STRING",
       "valueFrom": "arn:aws:secretsmanager:region:account-id:secret:MyAppSecrets:DbConnectionString::"
     }
   ]
   ```

### Option 3: AWS EKS (Elastic Kubernetes Service)
1. Create Kubernetes secrets or use AWS Secrets Manager with External Secrets Operator
2. Mount secrets as environment variables in pod specification
3. Example Kubernetes deployment:
   ```yaml
   env:
     - name: JWT_SECRET_KEY
       valueFrom:
         secretKeyRef:
           name: app-secrets
           key: jwt-secret-key
     - name: DATABASE_CONNECTION_STRING
       valueFrom:
         secretKeyRef:
           name: app-secrets
           key: database-connection-string
   ```

### Option 4: AWS Lambda (with API Gateway)
1. Package application for Lambda
2. Configure environment variables in Lambda function configuration
3. Use AWS Secrets Manager for sensitive values
4. Grant Lambda execution role permissions to access secrets

## AWS RDS Database Setup

### Recommended Configuration
1. Create an RDS SQL Server instance
2. Configure security groups to allow access from your application (ECS/EKS/EB)
3. Enable encryption at rest
4. Enable automated backups
5. Use Multi-AZ deployment for high availability

### Connection String Format
```
Server=<rds-endpoint>.rds.amazonaws.com,1433;Database=SampleDotNet6Db;User Id=<username>;Password=<password>;Encrypt=true;TrustServerCertificate=false;MultipleActiveResultSets=true;Connection Timeout=30;
```

## Security Best Practices

1. **Never commit secrets to source control**
2. **Use AWS Secrets Manager** for all sensitive configuration values
3. **Enable encryption** for data at rest and in transit
4. **Use IAM roles** for service-to-service authentication
5. **Enable CloudWatch logging** for monitoring and debugging
6. **Use VPC** to isolate your application and database
7. **Enable HTTPS** for all external communication
8. **Rotate secrets regularly** using AWS Secrets Manager rotation

## Health Check Endpoint
The application should expose a health check endpoint for AWS load balancers and container orchestration:
- Endpoint: `/health` or `/api/health`
- Configure in AWS ALB/NLB target group health checks

## Monitoring and Logging
- Application logs are written to console (stdout/stderr)
- AWS CloudWatch automatically captures container logs
- Configure CloudWatch alarms for critical metrics
- Use AWS X-Ray for distributed tracing (optional)

## Testing Cloud Configuration Locally

### Using Docker
```bash
docker run -e JWT_SECRET_KEY="your-test-key-at-least-32-characters-long" \
           -e DATABASE_CONNECTION_STRING="Server=localhost;Database=TestDb;User Id=sa;Password=YourPassword;" \
           -e JWT_ISSUER="SampleDotNet6App" \
           -e JWT_AUDIENCE="SampleDotNet6App" \
           -e CORS_ALLOWED_ORIGINS="http://localhost:3000" \
           -p 8080:80 \
           your-app-image:latest
```

### Using .NET CLI
```bash
export JWT_SECRET_KEY="your-test-key-at-least-32-characters-long"
export DATABASE_CONNECTION_STRING="Server=localhost;Database=TestDb;User Id=sa;Password=YourPassword;"
export JWT_ISSUER="SampleDotNet6App"
export JWT_AUDIENCE="SampleDotNet6App"
export CORS_ALLOWED_ORIGINS="http://localhost:3000"

dotnet run
```

## Troubleshooting

### Application fails to start with JWT error
- Ensure JWT_SECRET_KEY is set and is at least 32 characters long
- Check AWS Secrets Manager permissions if using secrets

### Database connection fails
- Verify DATABASE_CONNECTION_STRING is correctly formatted
- Check RDS security group allows inbound traffic from application
- Verify RDS instance is running and accessible
- Check VPC and subnet configuration

### CORS errors in browser
- Set CORS_ALLOWED_ORIGINS to include your frontend domain
- Ensure HTTPS is used in production
- Check that the origin matches exactly (including protocol and port)

## Migration from Development to Production

1. **Remove all hardcoded values** from appsettings.json (already done)
2. **Create AWS Secrets Manager secrets** for sensitive values
3. **Configure environment variables** in your AWS service (ECS/EKS/EB)
4. **Test the application** in a staging environment first
5. **Enable monitoring and logging** in CloudWatch
6. **Set up alarms** for critical metrics
7. **Configure auto-scaling** based on load
8. **Enable backup and disaster recovery** procedures

## Support
For issues or questions about cloud deployment, refer to:
- AWS Documentation: https://docs.aws.amazon.com/
- .NET on AWS: https://aws.amazon.com/developer/language/net/
