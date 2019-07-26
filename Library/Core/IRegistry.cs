namespace Pyro
{
    using System;
    using System.Text;
    using System.Collections.Generic;

    /// <summary>
    /// A mapping of id to object.
    ///
    /// Can be serialised to Json or binary.
    /// </summary>
    public interface IRegistry
    {
        /// <summary>
        /// Used for networking.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Add a new class to the registry.
        /// </summary>
        /// <param name="class">the class to add</param>
        /// <returns>true if class was added</returns>
        bool Register(IClassBase @class);

        /// <summary>
        /// Get a reference to given object specified by its id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IRefBase GetRef(Id id);

        /// <summary>
        /// Get a referenced to a typed object specified by its id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        IRef<T> GetRef<T>(Id id);

        /// <summary>
        /// Get a previously registered class by name.
        ///
        /// Used primarily for persistence.
        /// </summary>
        /// <param name="name">The name of the class</param>
        /// <returns>A class if it was found, else null</returns>
        IClassBase GetClass(string name);

        /// <summary>
        /// Get the underlying value of a given object. This is equivalent to derefenencing.
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <param name="obj">The generic object to dereference.</param>
        /// <returns>The deferenced typer</returns>
        T Get<T>(object obj);

        /// <summary>
        /// Try to dereference an object to a given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        bool TryGet<T>(object obj, out T val);

        /// <summary>
        /// Add a new object to the registry
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IRefBase Add(object value);

        /// <summary>
        /// Add a new object given a specific value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>A reference to the new reference object</returns>
        IRef<T> Add<T>(T value);
        IRef<T> Add<T>();

        /// <summary>
        /// Add a const-reference to the given value.
        /// </summary>
        /// <param name="val">The value to reference.</param>
        /// <returns>A const reference</returns>
        IConstRefBase AddConst(object val);

        /// <summary>
        /// Add a const-reference to given stronly-typed (reference) type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        IConstRef<T> AddConst<T>(T val);

        IConstRef<T> AddConst<T>();
        IClassBase GetClass(Type type);
        IClass<T> GetClass<T>();

        object New(IClassBase @class, Stack<object> dataStack);
        IRefBase NewRef(IClassBase @class, Stack<object> dataStack);
        IConstRefBase NewConstRef(IClassBase @class, Stack<object> dataStack);

        void ToPiScript(StringBuilder stringBuilder, object o);
        string ToPiScript(object obj);

        /// <summary>
        /// Create a new instance that has the same value as the given instance.
        /// </summary>
        /// <param name="obj">The object to duplicate.</param>
        /// <returns>A new new copy of original object.</returns>
        object Duplicate(object obj);
    }
}

