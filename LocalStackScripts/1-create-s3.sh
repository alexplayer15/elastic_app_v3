#!/usr/bin/env bash

awslocal s3api create-bucket \
  --bucket elastic-app-profile-pictures \
  --region eu-west-1 \
  --create-bucket-configuration LocationConstraint=eu-west-1
  
awslocal s3api put-bucket-cors \
  --bucket elastic-app-profile-pictures \
  --cors-configuration '{
    "CORSRules": [
      {
        "AllowedOrigins": ["http://localhost:5173"],
        "AllowedMethods": ["PUT"],
        "AllowedHeaders": ["*"],
        "MaxAgeSeconds": 3000
      }
    ]
  }'
  
echo "S3 Created Successfully"