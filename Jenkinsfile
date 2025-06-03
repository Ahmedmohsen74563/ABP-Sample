pipeline {
    agent any

    environment {
        SOLUTION = 'Acme.BookStore.sln'
        BUILD_CONFIGURATION = 'Release'
        ARTIFACT_NAME = "AcmeBookStore-${env.BUILD_NUMBER}"
        NPM_CACHE_FOLDER = "${env.WORKSPACE}\\.npm"
        ARTIFACT_DIR = "${env.WORKSPACE}\\publish"
        IIS_SITE_NAME = 'Jenkins'
        IIS_SITE_PATH = 'C:\\inetpub\\wwwroot\\Jenkins'
    }

    stages {
        
        stage('Restore Dependencies') {
            steps {
                echo "Restoring solution dependencies..."
                bat "dotnet restore ${SOLUTION}"
            }
        }

        stage('Build') {
            steps {
                echo "Building solution..."
                bat "dotnet build ${SOLUTION} --configuration ${BUILD_CONFIGURATION} --no-restore"
            }
        }

        stage('Publish') {
            steps {
                echo "Publishing solution..."
                bat "dotnet publish ${SOLUTION} --configuration ${BUILD_CONFIGURATION} --output ${ARTIFACT_DIR} --no-build"
                bat "powershell Compress-Archive -Path ${ARTIFACT_DIR}\\* -DestinationPath ${ARTIFACT_NAME}.zip"
            }
        }

        stage('Archive Artifacts') {
            steps {
                archiveArtifacts artifacts: "${ARTIFACT_NAME}.zip", fingerprint: true
            }
        }

        stage('Deploy to IIS') {
            steps {
                echo "Deploying to IIS..."

                // Optional: Take site offline (App_Offline)
                bat '''
                powershell -NoProfile -Command "$html = \'<html><body>Site is being updated...</body></html>\'; Set-Content -Path \\"%ARTIFACT_DIR%\\\\App_Offline.htm\\" -Value $html"
                copy %ARTIFACT_DIR%\\App_Offline.htm %IIS_SITE_PATH%\\App_Offline.htm
                '''
                // Clean existing site files
                bat "powershell Remove-Item -Recurse -Force ${IIS_SITE_PATH}\\*"

                // Unzip published app
                bat "powershell Expand-Archive -Path ${ARTIFACT_NAME}.zip -DestinationPath ${IIS_SITE_PATH}"

                // Ensure IIS site exists and is started
                bat """
                powershell -NoProfile -Command \"
                    Import-Module WebAdministration;
                    if (-Not (Test-Path IIS:\\Sites\\${IIS_SITE_NAME})) {
                        New-Website -Name '${IIS_SITE_NAME}' -Port 88 -PhysicalPath '${IIS_SITE_PATH}' -ApplicationPool 'DefaultAppPool';
                    }
                    Start-Website -Name '${IIS_SITE_NAME}';
                \"
                """
            }
        }

        stage('Clean Workspace') {
            steps {
                cleanWs()
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
