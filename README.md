# DockerRegistryClient

A sample registry client

## Install

```
dotnet tool install --global DockerRegistryClient.Cli --version 0.0.10
```

## Usage

```
Usage: drc [OPTIONS]

Options:
  -X       DELETE, REPO, TAG
  -R       Repository or registry address
  -U       User
  -P       Password

Examples:
  Delete image:    drc -X DELETE -R http://localhost:5000/test:latest
  List repository: drc -X REPO   -R http://localhost:5000
  List tag:        drc -X TAG    -R http://localhost:5000/test
```
