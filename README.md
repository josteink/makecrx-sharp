
# makecrx-sharp

makecrx-sharp is a cross-platform tool to package a directory as a Chrome-compatible
WebExtension in `*.crx`-format written in C#.

The packaged file is compliant with [CRX Package format v2](https://developer.chrome.com/extensions/crx).

## usage

In its most simple form, the following should suffice:

````
cd D:\Git\BrowserExtensionProject
makecrx /sourceDir:"Source"
dir *.crx
````

You can also specify a key using the `/key:key.pem` parameter. If key does not exist, a new one will be
created. If no `/key:` parameter is provided, a default name will be used.

## building

Buliding on Windows should be as simple as:

````
git clone https://github.com/josteink/makecrx-sharp
cd makecrx-sharp
nuget restore
msbuild
````

On Linux, you can just use [the officially supported scripts](https://developer.chrome.com/extensions/crx#scripts),
but you might also want to try to build this with mono using `xbuild` instead of `msbuild`.
