# PowerShell Script to Initialize Git Repository and Push to GitHub

# Check if git is installed
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Host "Git is not installed. Please install Git and try again." -ForegroundColor Red
    exit
}

Write-Host "Starting Git repository setup..."

# 1. Initialize Git
git init
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to initialize Git repository." -ForegroundColor Red
    exit
}
Write-Host "Git repository initialized." -ForegroundColor Green

# 2. Stage all files
git add .
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to stage files." -ForegroundColor Red
    exit
}
Write-Host "All files staged." -ForegroundColor Green

# 3. Initial Commit
git commit -m "Initial Commit: Project Structure"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to create initial commit." -ForegroundColor Red
    exit
}
Write-Host "Initial commit created successfully." -ForegroundColor Green

# 4. Ask for GitHub repository URL
$repoUrl = Read-Host -Prompt "Please enter the URL of your new GitHub repository"

if ([string]::IsNullOrWhiteSpace($repoUrl)) {
    Write-Host "Repository URL cannot be empty." -ForegroundColor Red
    exit
}

# 5. Add remote origin
git remote add origin $repoUrl
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to add remote origin. It might already exist." -ForegroundColor Yellow
    Write-Host "Attempting to set the URL instead."
    git remote set-url origin $repoUrl
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to set remote URL. Please check the URL and your Git configuration." -ForegroundColor Red
        exit
    }
}
Write-Host "Remote 'origin' added." -ForegroundColor Green

# 6. Push to main branch
Write-Host "Pushing to the main branch..."
git push -u origin main
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to push to the main branch. Please check your repository URL, credentials, and network connection." -ForegroundColor Red
    exit
}

Write-Host "Successfully pushed to the main branch on GitHub!" -ForegroundColor Green
Write-Host "Setup complete."
