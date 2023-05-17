#!/bin/sh

coverlet AwsWebsocketDotnetTemplate.Tests/bin/Debug/net6.0/AwsWebsocketDotnetTemplate.Tests.dll --target "dotnet" --targetargs "test AwsWebsocketDotnetTemplate.Tests --no-build"
