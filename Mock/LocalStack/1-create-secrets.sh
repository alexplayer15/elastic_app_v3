#!/bin/sh
set -e 
echo "Creating mock secrets"

awslocal secretsmanager create-secret \
    --name JwtConfig \
    --secret-string '{
        "PrivateKey": "BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA",
        "ExpirationInMinutes": 60,
        "Issuer": "PINEAPPLE",
        "Audience": "MANGO"
    }' \
    --region eu-west-2

echo "Mock secrets successfully created"