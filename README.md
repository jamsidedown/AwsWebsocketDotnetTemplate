# AwsWebsocketDotnetTemplate
Template for an AWS hosted websocket service using dotnet

## AWS services used
- CloudFormation
- Api Gateway
- Lambda
- DynamoDB

## Requirements
- AWS account 
- AWS CLI ([Installer](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)) ([Setup](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-configure.html))
- AWS SAM CLI ([Installer](https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/install-sam-cli.html))
- Dotnet SDK 6+ ([Installer](https://dotnet.microsoft.com/en-us/download))

## Architecture
![architecture diagram](docs/WebsocketAPI.drawio.svg)

- User connects to API Gateway via websockets (wss)
- API Gateway invokes Connect Lambda to store the user's unique connection id to DynamoDB
- When the user sends a message through the wss connection, the Default Lambda is invoked to handle the message
    - For now the Default Lambda will parrot the user's message back to the user
- When the user disconnects, the Disconnect Lambda runs to remove the user from DynamoDB
- A time to live (TTL) is configured in DynamoDB to clean up any connections that may have closed without triggering the Disconnect Lambda

## Running the tests
From the `src` directory, running `dotnet test` will run all unit tests

```sh
$ dotnet test
  Determining projects to restore...
  All projects are up-to-date for restore.
  AwsWebsocketDotnetTemplate -> ./AwsWebsocketDotnetTemplate/bin/Debug/net6.0/AwsWebsocketDotnetTemplate.dll
  AwsWebsocketDotnetTemplate.Tests -> ./AwsWebsocketDotnetTemplate.Tests/bin/Debug/net6.0/AwsWebsocketDotnetTemplate.Tests.dll
Test run for ./AwsWebsocketDotnetTemplate.Tests/bin/Debug/net6.0/AwsWebsocketDotnetTemplate.Tests.dll (.NETCoreApp,Version=v6.0)
Microsoft (R) Test Execution Command Line Tool Version 17.5.0 (arm64)
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1, Duration: < 1 ms - AwsWebsocketDotnetTemplate.Tests.dll (net6.0)
```

## Building and deploying
From the root directory, the SAM CLI can be used to build and deploy to AWS.

The first time a project is deployed it requires some additional parameters for the `sam deploy` command

```sh
# first run a build to compile and package the code
sam build

# then deploy to AWS
# the stack name can be replaced with whatever you choose
sam deploy --stack-name MyWebsocketApi --capabilities CAPABILITY_NAMED_IAM --guided
# leave all guided values as default
```

After the initial deploy, updates to the stack are simpler
```sh
sam build && sam deploy
```
