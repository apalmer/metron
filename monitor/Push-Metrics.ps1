param(
    $supabaseApiKey = (Get-Secret metron-superbase-key -AsPlainText),
    $supabaseUserEmail = (Get-Secret metron-supabase-powershell-email -AsPlainText),
    $supabaseUserPassword = (Get-Secret metron-supabase-powershell-password -AsPlainText)
    $supabaseProjectRef = 'azgiaiepgojvvafewtea' 
)

$headers = @{
    "apikey"       = "$supabaseApiKey";
    "Content-Type" = "application/json";
}

$baseUrl = "https://$supabaseProjectRef.supabase.co"

$tokenUrl = "$baseUrl/auth/v1/token?grant_type=password"
$credentials = @{ 
    "email" = $supabaseUserEmail; 
    "password" = $supabaseUSerPassword; 
} | ConvertTo-Json

$signIn = Invoke-RestMethod -Method Post -Uri $tokenUrl -Headers $headers -Body $credentials 
$token = $signIn.access_token

$headers['Authorization'] = "Bearer $token"

$ingestionUrl = "$baseUrl/rest/v1/ingestion"
$row = @{
    "data_schema_name"    = "ComputerInfo"
    "data_schema_version" = "0.0.1"
    "data"                = Get-ComputerInfo
} | ConvertTo-Json

$response = Invoke-RestMethod -Method Post -Uri $ingestionUrl -Headers $headers -Body $row
