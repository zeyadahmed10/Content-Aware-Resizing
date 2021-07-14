# Content-Aware-Resizing
Image resizing is a standard tool in many image processing applications. It works by uniformly resizing the image to a target size. However, this standard resizing does not consider the image content, leading to stretch/elongate the objects inside the image


# Content-Aware-Resizing Idea
Content-aware resizing mainly uses an energy function to define the importance of pixels. The pixels with low energy are less important and can be eliminated when resizing the image.

In this repo. you will find a simple implementation for Content-Aware-Resizing using Dynamic Programming.
