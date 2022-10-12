# Wayland C#

A Wayland Client proof of concept written in c# without P/Invoking libwayland.

A downside of not using libwayland is that the presentation engine for Vulkan and Opengl has to be implemented in app instead of using the one provided by the driver.

Opengl Es - Works <br />
Opengl - Not Implemented <br />
Vulkan - Not Implemented <br />

Issues:
1. Event Queue not Implemented properly.
2. Frames are being skipped.