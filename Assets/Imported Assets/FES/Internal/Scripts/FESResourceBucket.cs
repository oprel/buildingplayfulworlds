namespace FESInternal
{
    using UnityEngine;

    /// <summary>
    /// A bucket that holds references to internal FES resources
    /// </summary>
    public class FESResourceBucket : MonoBehaviour
    {
        /// <summary>
        /// List of all internal texture resources
        /// </summary>
        public Texture2D[] Textures;

        /// <summary>
        /// List of all internal material resources
        /// </summary>
        public Material[] Materials;

        /// <summary>
        /// Gets Texture2D with the given name
        /// </summary>
        /// <param name="name">Name of texture</param>
        /// <returns>Texture or null if not found</returns>
        public Texture2D LoadTexture2D(string name)
        {
            if (Textures != null)
            {
                foreach (var texture in Textures)
                {
                    if (texture != null && texture.name == name)
                    {
                        return texture;
                    }
                }
            }

            Debug.Log("Could not find texture: " + name);

            return null;
        }

        /// <summary>
        /// Gets Material with the given name
        /// </summary>
        /// <param name="name">Name of material</param>
        /// <returns>Material or null if not found</returns>
        public Material LoadMaterial(string name)
        {
            if (Materials != null)
            {
                foreach (var material in Materials)
                {
                    if (material != null && material.name != null)
                    {
                        if (material.name.Split(' ')[0] == name)
                        {
                            return material;
                        }
                    }
                }
            }

            Debug.Log("Could not find material: " + name);

            return null;
        }
    }
}
