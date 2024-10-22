pipeline {
      agent {
        docker {
            image 'mcr.microsoft.com/playwright:v1.42.1-jammy' // Playwright Docker image
            args '-u root:root' // Optional: run as root user
        }
    }
    
    parameters {
        string(name: 'PROJECT_KEY', description: 'The key of project', defaultValue: '')
        string(name: 'TEST_EXECUTION_KEY', description: 'The key of test execution', defaultValue: '')
    }

    stages {
        stage('Checkout code') {
            steps {
                checkout scm
            }
        }

        stage('Install dependencies') {
            steps {
                sh 'npm ci'
            }
        }

        stage('Run tests') {
            steps {
                sh 'npm run test'
            }
        }

        stage('Authenticate and upload test results') {
            steps {
                script {
                    // Authenticate and get the token
                    def response = sh(script: """
                        curl -X POST 'https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate' \
                        -H 'Content-Type: application/json' \
                        --data '{"clientId":"${env.CLIENT_ID}","clientSecret":"${env.CLIENT_SECRET}"}'
                    """, returnStdout: true).trim()
                    def token = response.token // Adjust as needed to parse the token

                    // Upload the test results
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

        stage('Set result based on exit code') {
            steps {
                script {
                    // Determine the result based on the exit code
                    if (currentBuild.result == null) {
                        currentBuild.result = 'SUCCESS'
                    } else {
                        currentBuild.result = 'FAILURE'
                    }
                }
            }
        }

        stage('Send test execution result') {
            steps {
                script {
                    // Authenticate and get the token again
                    def response = sh(script: """
                        curl -H "Content-Type: application/json" -X POST --data '{ "clientId": "${env.CLIENT_ID}", "clientSecret": "${env.CLIENT_SECRET}" }' https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate
                    """, returnStdout: true).trim()
                    def token = response.token // Adjust as needed to parse the token

                    // Send the test execution result
                    sh """
                        curl -H "Content-Type: application/json" -H "Authorization: JWT ${token}" --data '{ "jobURL": "${env.BUILD_URL}", "tool": "jenkins", "result": "${currentBuild.result}" }' "https://dev.agiletest.atlas.devsamurai.com/ds/test-executions/${params.TEST_EXECUTION_KEY}/pipeline/history?projectKey=${params.PROJECT_KEY}"
                    """
                }
            }
        }

        stage('Echo values for debugging') {
            steps {
                script {
                    echo "Job URL: ${env.BUILD_URL}"
                }
            }
        }
    }
    
    environment {
        CLIENT_ID = credentials('CLIENT_ID') // Assuming you have a Jenkins credential set up
        CLIENT_SECRET = credentials('CLIENT_SECRET') // Assuming you have a Jenkins credential set up
    }
}
