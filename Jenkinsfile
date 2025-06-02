pipeline {
    agent any

    environment {
        BUILD_CONFIGURATION = 'Release'
        ARTIFACT_NAME = 'dotnet9app'
        NPM_CACHE_FOLDER = "${env.WORKSPACE}\\.npm"
        ARTIFACT_DIR = "${env.WORKSPACE}\\publish"
    }

    stages {
        stage('Restore Dependencies') {
            steps {
                echo "Restoring all .csproj files..."
                bat 'powershell -Command "Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object { dotnet restore $_.FullName }"'
            }
        }

        stage('Build') {
            steps {
                echo "Building all .csproj files..."
                bat 'powershell -Command "Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object { dotnet build $_.FullName --configuration %BUILD_CONFIGURATION% }"'
            }
        }

        stage('Publish') {
            steps {
                echo "Publishing all .csproj files..."
                bat 'powershell -Command "Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object { dotnet publish $_.FullName --configuration %BUILD_CONFIGURATION% --output %ARTIFACT_DIR% }"'
                bat "powershell Compress-Archive -Path %ARTIFACT_DIR%\\* -DestinationPath %ARTIFACT_NAME%.zip"
            }
        }

        stage('Archive Artifacts') {
            steps {
                archiveArtifacts artifacts: "${ARTIFACT_NAME}.zip", fingerprint: true
            }
        }
    }

    post {
        failure {
            echo 'Build failed.'
        }
        success {
            echo 'Build succeeded.'
        }
    }
}
