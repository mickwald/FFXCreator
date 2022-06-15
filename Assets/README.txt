First steps to using this asset is to create an object to create an effect for. We recommend a plane.
Second, add the ShaderScript asset to your newly created object.
Now, the script needs a reference to the shader to be able to upload data to the GPU. 
Do this by dragging the TextureShader shader-asset to the correct reference slot.
We are now almost ready to start creating an effect. It is possible to use an existing material for this, but any material applied to the 
script will be commandeered for this use. If you want multiple geometry using the same effect, that is possible, but then only apply the 
ShaderScript to a "main" object to handle the material and manually add the material to secondary objects.
Apply a material to the ShaderScript.
You can now start creating your scrolling texture effect. Use the supplied controls in the shader script.


Albedo for the material is calculated by multiplying the texture value with Layer Weight and then dividing by the total layer weight across 
all layers, thus, a layer with weight 0 will not show up at all and for example two layers with weight 1 (with no other layers)
will show as a 50/50 split.

Alpha is calculated in the same way as albedo.

Looking around in the scene in the editor (right-click) will allow you to get a good look of how the effect looks in-game.