
$loginUrl = "http://localhost:5282/api/Auth/login"
$meUrl = "http://localhost:5282/api/Auth/me"
$body = @{
    username = "admin"
    password = "Admin@123"
} | ConvertTo-Json

try {
    echo "Logging in..."
    $loginResponse = Invoke-RestMethod -Uri $loginUrl -Method Post -ContentType "application/json" -Body $body
    
    if ($loginResponse.success) {
        $token = $loginResponse.data.accessToken
        echo "Login successful. Token received."
        # echo "Token: $token" 

        echo "Calling /api/Auth/me..."
        $headers = @{
            Authorization = "Bearer $token"
        }
        
        try {
            $meResponse = Invoke-RestMethod -Uri $meUrl -Method Get -Headers $headers
            echo "Me Response:"
            echo ($meResponse | ConvertTo-Json -Depth 5)
        } catch {
            echo "Error calling /api/Auth/me"
            echo $_.Exception.Message
        }

        $adminUrl = "http://localhost:5282/api/Auth/admin-test"
        echo "Calling /api/Auth/admin-test..."
        try {
            $adminResponse = Invoke-RestMethod -Uri $adminUrl -Method Get -Headers $headers
            echo "Admin Response:"
            echo ($adminResponse | ConvertTo-Json -Depth 5)
        } catch {
            echo "Error calling /api/Auth/admin-test"
            echo $_.Exception.Message
            if ($_.Exception.Response) {
                $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
                echo "Response Body: $($reader.ReadToEnd())"
            }
        }
    } else {
        echo "Login failed: $($loginResponse.message)"
    }
} catch {
    echo "Error logging in"
    echo $_.Exception.Message
    if ($_.Exception.Response) {
        $reader = [System.IO.StreamReader]::new($_.Exception.Response.GetResponseStream())
        echo "Response Body: $($reader.ReadToEnd())"
    }
}
