/// <summary>
/// Defines the communication for controlling robot
/// </summary>

using UnityEngine;

interface ComController
{
    /// <summary>
    /// Select this robot
    /// </summary>
    void select();

    /// <summary>
    /// Unselect this robot
    /// </summary>
    void unselect();

    /// <summary>
    /// Follow a GameObject
    /// </summary>
    /// <param name="g">The GameObject to follow</param>
    void follow(GameObject g);

    /// <summary>
    /// Avoid a GameObject
    /// </summary>
    /// <param name="g">The GameObject to avoid</param>
    void avoid(GameObject g);

    /// <summary>
    /// GoTo a GameObject
    /// </summary>
    /// <param name="g">The GameObject to go</param>
    void goTo(GameObject g);

    /// <summary>
    /// Group some robot into specific scheme
    /// </summary>
    /// <param name="scheme">The scheme to use</param>
    void group(string scheme);

    /// <summary>
    /// Get a signal from a robot
    /// </summary>
    /// <returns>The signal to return</returns>
    int sensorSignal();
}
