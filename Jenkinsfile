pipeline {
    agent any

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

        stage('Run tests in Playwright Docker container') {
            steps {
                script {
                    // Run Playwright tests inside the Docker container
                    sh '''
                        docker run --rm -v "$PWD:/app" -w /app mcr.microsoft.com/playwright:v1.42.1-focal /bin/bash -c "
                            npm ci && npm run test
                        "
                    '''
                }
            }
        }

        stage('Authenticate and upload test results') {
            steps {
                script {
                    def token = sh(script: """
                        curl -X POST 'https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate' \
                        -H 'Content-Type: application/json' \
                        --data '{"clientId":"${env.CLIENT_ID}","clientSecret":"${env.CLIENT_SECRET}"}'
                    """, returnStdout: true).trim()

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
                    def token = sh(script: """
                        curl -H "Content-Type: application/json" -X POST --data '{ "clientId": "${env.CLIENT_ID}", "clientSecret": "${env.CLIENT_SECRET}" }' https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate
                    """, returnStdout: true).trim()

                    sh """
                        curl -H "Content-Type: application/json" -H "Authorization: JWT ${token}" --data '{ "jobURL": "${env.BUILD_URL}", "tool": "jenkins", "result": "${currentBuild.result}" }' "https://dev.agiletest.atlas.devsamurai.com/ds/test-executions/${params.TEST_EXECUTION_KEY}/pipeline/history?projectKey=${params.PROJECT_KEY}"
                    """
                }
            }
        }
    }

    environment {
        CLIENT_ID = credentials('CLIENT_ID') // Ensure Jenkins credentials are configured
        CLIENT_SECRET = credentials('CLIENT_SECRET') // Ensure Jenkins credentials are configured
    }
}
