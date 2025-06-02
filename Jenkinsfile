pipeline {
    agent any

    environment {
        SOLUTION = '**\\*.csproj'
        BUILD_CONFIGURATION = 'Release'
        ARTIFACT_NAME = 'dotnet9app'
        NPM_CACHE_FOLDER = "${env.WORKSPACE}\\.npm"
        ARTIFACT_DIR = "${env.WORKSPACE}\\publish"
    }

    stages {
        stage('Restore Dependencies') {
            steps {
                echo "Restoring projects: ${env.SOLUTION}"
                bat "dotnet restore ${env.SOLUTION}"
            }
        }

        stage('Build') {
            steps {
                echo "Building with configuration: ${env.BUILD_CONFIGURATION}"
                bat "dotnet build ${env.SOLUTION} --configuration %BUILD_CONFIGURATION%"
            }
        }

        stage('Publish') {
            steps {
                echo "Publishing projects..."
                bat "dotnet publish ${env.SOLUTION} --configuration %BUILD_CONFIGURATION% --output %ARTIFACT_DIR%"
                // Optional: zip the published output
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
