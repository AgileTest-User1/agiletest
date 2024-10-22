pipeline {
    agent
    { docker { image 'mcr.microsoft.com/playwright:v1.48.1-noble' } }
   
   stages {
      stage('e2e-tests') {
         steps {
            sh 'npm ci'
            sh 'npx playwright test'
         }
      }
   }


    parameters {
        string(name: 'PROJECT_KEY', description: 'The key of project', defaultValue: 'AUT')
        string(name: 'TEST_EXECUTION_KEY', description: 'The key of test execution', defaultValue: 'AUT-3879')
    }

    environment {
        CLIENT_ID = 'Mmar0YgnY3LFQD7I3AqlwEQ95xJ1i0Le0GVy49f1wcc=' 
        CLIENT_SECRET = 'dc6c48806069f4f8c2442076bdc806cc81170aa9aefa91a51eff979e5515b5d7' 
    }

    stages {
        stage('Checkout code') {
            steps {
                checkout scm
            }
        }


          stages {
      stage('e2e-tests') {
         steps {
            sh 'npm ci'
            sh 'npm run test'
         }
      }
   }


        stage('Authenticate and upload test results') {
            steps {
                script {
                    def response = sh(script: """
                        curl -X POST 'https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate' \
                        -H 'Content-Type: application/json' \
                        --data '{"clientId":"${env.CLIENT_ID}","clientSecret":"${env.CLIENT_SECRET}"}'
                    """, returnStdout: true).trim()
                    
                    // Assuming the response contains a JSON with a token field
                    def token = new groovy.json.JsonSlurper().parseText(response).token
                    
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
                    // Automatically setting result based on test execution
                    if (currentBuild.result == null || currentBuild.result == 'SUCCESS') {
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
