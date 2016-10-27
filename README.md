
# makecrx-sharp

makecrx-sharp is a cross-platform tool to package a directory as a Chrome-compatible
WebExtension in `*.crx`-format written in C#.

The packaged file is compliant with [CRX Package format v2](https://developer.chrome.com/extensions/crx).

The canonical source for this project is https://github.com/josteink/makecrx-sharp.

## usage

In its most simple form, downloading the [latest release](https://github.com/josteink/makecrx-sharp/releases) and issuing the following commands should suffice:

````
cd D:\Git\BrowserExtensionProject
makecrx /sourceDir:"Source"
dir *.crx
````

You can also specify a key using the `/key:key.pem` parameter. If key does not exist, a new one will be
created. If no `/key:` parameter is provided, a default name will be used.

## features and limitations

This tool merely packages the content of an existing folder in compliance with the CRX Package format specification.

It does not verify the correctness of the package-content itself. For such purposes you should use other existing tools
like [web-ext](https://github.com/mozilla/web-ext) and [eslint](https://github.com/eslint/eslint).

## building

Building on Windows should be as simple as:

````
git clone https://github.com/josteink/makecrx-sharp
cd makecrx-sharp
nuget restore
msbuild
````

On Linux, you can just use [the officially supported scripts](https://developer.chrome.com/extensions/crx#scripts),
but you might also want to try to build this with mono using `xbuild` instead of `msbuild` (it's tested and works on
Ubuntu 16.04).

## disclaimer

This tool is provided as is with no warranties.
