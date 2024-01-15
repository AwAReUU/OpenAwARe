# Tests
> Unit and Unity tests

These folders contain all of our tests.


## Editmode tests

Contains standard unit tests. Callback methods of monobehaviours cannot be used here. 

## Playmode tests

Contains tests that can be run in a simulation. These are run in the application's runtime environment, and can therefor make use of callback methods of monobehaviours.

## Run Tests

Generating new code coverage reports is done via the Unity editor. In the navigation bar at the top, press window -> analysis -> Code coverage. This will open up a code coverage window. The test runner needs to be opened too. To do this, head over to window -> general -> test runner.

To actually generate a report, press the start recording button in the code coverage window. After this, run all tests that you want to include in the coverage report. Next, stop the recording. It should start generating a report. If this it is finished, the corresponding folder will open.

## Test code coverage

To find out more about code coverage, head over to [Code Coverage](https://github.com/Mackthis/AwARe/tree/main/AwARe/CodeCoverage)

## CI/CD pipeline

* [Lacking pipeline](https://docs.google.com/document/d/1J8v_kLuo3Qj6CKccRga5TKI3_PSqjla7v7AQFSO_H_4/edit) See this link for an explanation of the missing CI/CD github pipeline.