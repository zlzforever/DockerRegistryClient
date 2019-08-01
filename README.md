# DockerRegistryClient

A sample registry client

## Install

```
curl -L https://github.com/zlzforever/DockerRegistryClient/releases/download/0.0.10/drc-`uname -s`-`uname -m` -o /usr/local/bin/drc
chmod +x /usr/local/bin/drc
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
