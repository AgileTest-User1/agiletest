pipeline {
    agent {
        docker { image 'mcr.microsoft.com/playwright:v1.48.1-noble' }
    }

    parameters {
        string(name: 'PROJECT_KEY', description: 'The key of the project', defaultValue: 'AUT')
        string(name: 'TEST_EXECUTION_KEY', description: 'The key of the test execution', defaultValue: 'AUT-3879')
    }

    environment {
        CLIENT_ID = credentials('CLIENT_ID')       // Ensure Jenkins credentials are configured
        CLIENT_SECRET = credentials('CLIENT_SECRET') // Ensure Jenkins credentials are configured
    }

    stages {
        stage('Checkout Code') {
            steps {
                checkout scm
            }
        }

        stage('Install Dependencies') {
            steps {
                sh 'npm ci'
            }
        }

        stage('Run Playwright Tests') {
            steps {
                sh 'npm run test'
            }
        }

        stage('Authenticate and Upload Test Results') {
            steps {
                script {
                    // Authenticate and get the token
                    def response = sh(script: """
                        curl -X POST 'https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate' \
                        -H 'Content-Type: application/json' \
                        --data '{"clientId":"${env.CLIENT_ID}","clientSecret":"${env.CLIENT_SECRET}"}'
                    """, returnStdout: true).trim()
                    
                    def token = new groovy.json.JsonSlurper().parseText(response).token
                    
                    // Upload test results
                    sh """
                        curl -X POST \
                        -H "Content-Type: application/xml" \
                        -H "Authorization: JWT ${token}" \
                        --data @"./playwright-report/results.xml" \
                        "https://dev.api.agiletest.app/ds/test-executions/junit?projectKey=${params.PROJECT_KEY}&testExecutionKey=${params.TEST_EXECUTION_KEY}"
                    """
                }
            }
        }

        stage('Set Result Based on Exit Code') {
            steps {
                script {
                    // Automatically set the result based on test execution
                    if (currentBuild.result == null || currentBuild.result == 'SUCCESS') {
                        currentBuild.result = 'SUCCESS'
                    } else {
                        currentBuild.result = 'FAILURE'
                    }
                }
            }
        }

        stage('Send Test Execution Result') {
            steps {
                script {
                    def response = sh(script: """
                        curl -H "Content-Type: application/json" -X POST --data '{ "clientId": "${env.CLIENT_ID}", "clientSecret": "${env.CLIENT_SECRET}" }' https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate
                    """, returnStdout: true).trim()

                    def token = new groovy.json.JsonSlurper().parseText(response).token

                    sh """
                        curl -H "Content-Type: application/json" \
                        -H "Authorization: JWT ${token}" \
                        --data '{ "jobURL": "${env.BUILD_URL}", "tool": "jenkins", "result": "${currentBuild.result}" }' \
                        "https://dev.agiletest.atlas.devsamurai.com/ds/test-executions/${params.TEST_EXECUTION_KEY}/pipeline/history?projectKey=${params.PROJECT_KEY}"
                    """
                }
            }
        }
    }
}
