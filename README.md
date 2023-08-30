# Example of converting image file

## Configuration

Create appsettings.Development.json file on src/Example.ImageConverter.App directory.

- `App__Source`: Directory which images files placed
- `App__Filter`: Filter of image file to convert

```json
{
  "App": {
    "Source": "/path/to/images/directory",
    "Filter": "*.png"
  }
}
```

> - Find ` .png`` file from  `/path/to/images/directory` directory then converts them.
> - Output is `/path/to/images/directory/output`

## Run

Please use `.NET Core Launch (console)` on your vscode Run and debug.
