/* Dictionary.cs
 * Author: Rod Howell
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KansasStateUniversity.TreeViewer2;
using Ksu.Cis300.ImmutableBinaryTrees;

namespace Ksu.Cis300.NameLookup
{
    /// <summary>
    /// A generic dictionary in which keys must implement IComparable.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class Dictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// The keys and values in the dictionary.
        /// </summary>
        private BinaryTreeNode<KeyValuePair<TKey, TValue>> _elements = null;

        /// <summary>
        /// Gets a drawing of the underlying binary search tree.
        /// </summary>
        public TreeForm Drawing => new TreeForm(_elements, 100);

        /// <summary>
        /// Checks to see if the given key is null, and if so, throws an
        /// ArgumentNullException.
        /// </summary>
        /// <param name="key">The key to check.</param>
        private static void CheckKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Finds the given key in the given binary search tree.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="t">The binary search tree.</param>
        /// <returns>The node containing key, or null if the key is not found.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Find(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t)
        {
            if (t == null)
            {
                return null;
            }
            else
            {
                int comp = key.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    return t;
                }
                else if (comp < 0)
                {
                    return Find(key, t.LeftChild);
                }
                else
                {
                    return Find(key, t.RightChild);
                }
            }
        }

        /// <summary>
        /// Builds the binary search tree that results from adding the given key and value to the given tree.
        /// If the tree already contains the given key, throws an ArgumentException.
        /// </summary>
        /// <param name="t">The binary search tree.</param>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        /// <returns>The binary search tree that results from adding k and v to t.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Add(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, TKey k, TValue v)
        {
            if (t == null)
            {
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(k, v), null, null);
            }
            else
            {
                int comp = k.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    throw new ArgumentException();
                }
                else if (comp < 0)
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, Add(t.LeftChild, k, v), t.RightChild);
                }
                else
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, Add(t.RightChild, k, v));
                }
            }
        }

        /// <summary>
        /// Tries to get the value associated with the given key.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value associated with k, or the default value if
        /// k is not in the dictionary.</param>
        /// <returns>Whether k was found as a key in the dictionary.</returns>
        public bool TryGetValue(TKey k, out TValue v)
        {
            CheckKey(k);
            BinaryTreeNode<KeyValuePair<TKey, TValue>> p = Find(k, _elements);
            if (p == null)
            {
                v = default(TValue);
                return false;
            }
            else
            {
                v = p.Data.Value;
                return true;
            }
        }

        /// <summary>
        /// Adds the given key with the given associated value.
        /// If the given key is already in the dictionary, throws an
        /// InvalidOperationException.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        public void Add(TKey k, TValue v)
        {
            CheckKey(k);
            _elements = Add(_elements, k, v);
        }
        /// <summary>
        /// Method updates tree with minimum removed 
        /// </summary>
        /// <param name="t">Passes in the tree</param>
        /// <param name="min">Returns the keyvaluepair of the min removed</param>
        /// <returns></returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> RemoveMininumKey(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out KeyValuePair<TKey, TValue> min)
        {
            //If tree has a leftChild 
            if (t.LeftChild != null)
            {   //Recursively call the RemoveMinimum method to remove minimum of the left child tree
                BinaryTreeNode<KeyValuePair<TKey,TValue>> result = RemoveMininumKey(t.LeftChild,out min);
          
                //Return the updated tree of the leftChild 
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, result,t.RightChild); 
            }
            else
            {
                min = t.Data;
                //Return the tree's updated RightChild
                return t.RightChild; 
            }

        }
        /// <summary>
        /// Returns the result of removing the node containing the key 
        /// </summary>
        /// <param name="key">Passes in the key</param>
        /// <param name="t">Passes in the tree</param>
        /// <param name="removed">Returns whether the key was found in the tree</param>
        /// <returns></returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Remove(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out bool removed)
        {
            //If key does not exist
            if (t == null)
            {

                //Removed == false b/c the key was not found
                removed = false;
                //Return null for the tree
                return null;
                
            }
         else {
                int comp = key.CompareTo(t.Data.Key); 
                //If tree's data is less than key 
                if (comp > 0)
                {
                    //Recursively call Remove w/ tree.RightChild
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> rightUpdate =  Remove(key, t.RightChild, out removed);
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, rightUpdate);
                }
                else if (comp < 0)
                {
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> leftUpdate = Remove(key, t.LeftChild, out removed);
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data,leftUpdate, t.RightChild);
                }
                //If you have found the tree
                else
                {
                    removed = true; 
                    //If tree has no children 

                    //If tree has just ONE child
                     if ((t.LeftChild == null))
                    {
                        //Return that child
                        return t.RightChild;
                            

                    }
                    else if ((t.RightChild == null ))
                    {
                        //Return that child
                        return t.LeftChild; 
                    }
                    //If tree has both children
                    else   {
                        BinaryTreeNode<KeyValuePair<TKey, TValue>> updatedRight = RemoveMininumKey(t.RightChild, out KeyValuePair<TKey, TValue> min);
                        
                        return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(min, t.LeftChild, updatedRight);
                    }
                }

            }
        }
        /// <summary>
        /// Removes the given key and its associated value from dictionary using 'Remove' method
        /// </summary>
        /// <param name="k">Passes in the key</param>
        /// <returns></returns>
        public bool Remove(TKey k)
        {
            CheckKey(k);
            //sets up a bool variable for the out param
            bool result = false;
            //Call Remove with the overall (field) tree, key, and out param
            _elements = Remove(k, _elements, out result);
            return result; 
        }

    }
}
