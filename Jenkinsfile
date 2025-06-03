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
                bat "powershell Compress-Archive -Path ${ARTIFACT_DIR}\\* -DestinationPath ${ARTIFACT_NAME}.zip -Force"
            }
        }

        stage('Publish and Run Migrator') {
            steps {
                script {
                    def outputDir = "${env.ARTIFACT_DIR}\\migrator"
                    def migratorProject = 'src\\Acme.BookStore.DbMigrator\\Acme.BookStore.DbMigrator.csproj'
                    bat """
                    echo Installing EF CLI tools if needed...
                    dotnet tool install --global dotnet-ef --ignore-failed-sources
                    echo Adding EF tools path to environment...
                    set PATH=%PATH%;%USERPROFILE%\\.dotnet\\tools
                    echo Publishing DbMigrator project...
                    dotnet publish ${migratorProject} --configuration ${BUILD_CONFIGURATION} --output ${outputDir}
                    echo Running EF Core migrations...
                    dotnet ef database update --configuration ${BUILD_CONFIGURATION} --project ${migratorProject} --startup-project ${migratorProject}
                    """
                }
            }
        }
        stage('Deploy to IIS') {
            steps {
                echo "Deploying to IIS..."

                bat '''
                powershell -NoProfile -Command "$html = '<html><body>Site is being updated...</body></html>'; Set-Content -Path \\"%ARTIFACT_DIR%\\\\App_Offline.htm\\" -Value $html"
                copy "%ARTIFACT_DIR%\\App_Offline.htm" "%IIS_SITE_PATH%\\App_Offline.htm"
                '''

                bat "powershell Expand-Archive -Path ${ARTIFACT_NAME}.zip -DestinationPath ${IIS_SITE_PATH} -Force"
                bat "powershell Remove-Item -Path '${IIS_SITE_PATH}\\App_Offline.htm' -Force -ErrorAction SilentlyContinue"

                powershell '''
                Import-Module WebAdministration
                if (-Not (Test-Path IIS:\\Sites\\Jenkins)) {
                    New-Website -Name "Jenkins" -Port 88 -PhysicalPath "C:\\inetpub\\wwwroot\\Jenkins" -ApplicationPool "DefaultAppPool"
                }
                Start-Website -Name "Jenkins"
                '''
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
            echo 'Pipeline failed.'
        }
        success {
            echo 'Pipeline succeeded.'
        }
    }
}
