pipeline {
    agent any

    parameters {
        string(name: 'PROJECT_KEY', description: 'The key of project', defaultValue: '${PROJECT_KEY}')
        string(name: 'TEST_EXECUTION_KEY', description: 'The key of test execution', defaultValue: '${TEST_EXECUTION_KEY}')
        string(name: 'CLIENT_ID', description: 'The key of project', defaultValue: 'Mmar0YgnY3LFQD7I3AqlwEQ95xJ1i0Le0GVy49f1wcc=')
        string(name: 'CLIENT_SECRET', description: 'The key of test execution', defaultValue: 'dc6c48806069f4f8c2442076bdc806cc81170aa9aefa91a51eff979e5515b5d7')
        string(name: 'TRIGGER_PARAM', defaultValue: '11f60f1a919e539614c047910b2cbd99bf', description: 'Parameter for remote trigger')

    }

  stages {
        stage('Trigger Remote Build') {
            steps {
                script {
                    // Curl command to trigger remote build
                    sh '''
                        curl 'https://05e4-1-53-68-97.ngrok-free.app/job/Multi-branch/job/junit/buildWithParameters?PROJECT_KEY=AUT&TEST_EXECUTION_KEY=AUT-3879&delay=0sec' \
                        -H 'sec-ch-ua-platform: "macOS"' \
                        -H 'Authorization: Basic TmFuYToxMWY2MGYxYTkxOWU1Mzk2MTRjMDQ3OTEwYjJjYmQ5OWJm' \
                        -H 'Referer: https://dev.agiletest.atlas.devsamurai.com/' \
                        -H 'sec-ch-ua: "Google Chrome";v="129", "Not=A?Brand";v="8", "Chromium";v="129"' \
                        -H 'sec-ch-ua-mobile: ?0' \
                        -H 'User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36' \
                        -H 'Accept: application/json, text/plain, */*' \
                        -H 'Content-Type: application/json' \
                        --data-raw '{}'
                    '''
                }
            }
            }
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
