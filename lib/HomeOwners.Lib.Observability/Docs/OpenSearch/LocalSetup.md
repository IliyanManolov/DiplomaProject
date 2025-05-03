# Initial setup for local instance of OpenSearch
The following document describes the steps for setting up your local instance of OpenSearch for tracking logs in an easier matter.

**NOTE**: OpenSearch is completely optional and the application is not hard-coupled to it.


After starting the docker-compose project for the first time you will need to head to the [saved objects](http://localhost:5601/app/management/opensearch-dashboards/objects) page of OpenSearch and import the [dashboard](./dashboardExport.ndjson) itself located in the folder with this document.

If prompted to log-in - use the account `admin` and the local password in the `.env` file

After importing we need to manually setup the template and data stream(s).
This takes a bit longer than using normal indexes but is aimed at mimicking a production environment due to datastreams NOT being meant for editing - as is the case with our application logs


- Head over to the [template creation](http://localhost:5601/app/opensearch_index_management_dashboards#/create-template) page:
  - Give the template any name. Preferable `homeowners-logs-template`
  - Make sure the `Template type` is set to "**Data streams**"
  - set the `Time field` to "**ts**"
  - Set the `Index patterns` to "homeowners-*-logs"
  - At the bottom select the `JSON Editor` for the Index mappings and paste the following JSON:
  -
    ```json
    {
        "properties": {
            "ts": {
                "format": "[yyyy-MM-dd'T'HH:mm:ss:SSS]",
                "type": "date"
            },
            "v": {
                "fields": {
                    "keyword": {
                        "type": "keyword"
                    }
                },
                "type": "text"
            },
            "ctx": {
                "fields": {
                    "keyword": {
                        "type": "keyword"
                    }
                },
                "type": "text"
            },
            "err": {
                "fields": {
                    "keyword": {
                        "type": "keyword"
                    }
                },
                "type": "text"
            },
            "errType": {
                "fields": {
                    "keyword": {
                        "type": "keyword"
                    }
                },
                "type": "text"
            },
            "innerEx": {
                "type": "object",
                "properties": {
                    "err": {
                        "fields": {
                            "keyword": {
                                "type": "keyword"
                            }
                        },
                        "type": "text"
                    },
                    "errType": {
                        "fields": {
                            "keyword": {
                                "type": "keyword"
                            }
                        },
                        "type": "text"
                    },
                    "st": {
                        "fields": {
                            "keyword": {
                                "type": "keyword"
                            }
                        },
                        "type": "text"
                    }
                }
            },
            "msg": {
                "fields": {
                    "keyword": {
                        "type": "keyword"
                    }
                },
                "type": "text"
            },
            "st": {
                "fields": {
                    "keyword": {
                        "type": "keyword"
                    }
                },
                "type": "text"
            },
            "tr": {
                "fields": {
                    "keyword": {
                        "type": "keyword"
                    }
                },
                "type": "text"
            },
            "prop": {
                "type": "object",
                "properties": {
                    "name": {
                        "fields": {
                            "keyword": {
                                "type": "keyword"
                            }
                        },
                        "type": "text"
                    },
                    "value": {
                        "fields": {
                            "keyword": {
                                "type": "keyword"
                            }
                        },
                        "type": "text"
                    }
                }
            },
            "propLong": {
                "type": "object",
                "properties": {
                    "name": {
                        "type": "text"
                    },
                    "value": {
                        "type": "text"
                    }
                }
            }
        }
    }
    ```
  - The rest of the options such as alias, number of shards, etc. are not needed for the local setup


- After we have created the template itself we need to create the [data streams](http://localhost:5601/app/opensearch_index_management_dashboards#/create-data-stream) themselves
  - `homeowners-api-logs`
  - `homeowners-web-logs`
- Upon filling in the name the mapping we made previously should appear. If not, re-check the matching pattern on the template.
- The final step is refreshing the [index patterns](http://localhost:5601/app/management/opensearch-dashboards/indexPatterns)
  - Go into each one and refresh the `field list` (top right) - this is done in case the API made a request and OpenSearch created an index pattern by itself before we did this setup.
  - **NOTE**: If there are no index patterns visibiel you need to import the dashboards and all additional elements as described at the start of the document.