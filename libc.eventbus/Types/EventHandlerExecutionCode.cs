namespace libc.eventbus.Types {
    /// <summary>
    /// Execution codes of executing a handler
    /// </summary>
    public enum EventHandlerExecutionCode {
        /// <summary>
        /// Handler is executed
        /// </summary>
        Executed = 0,

        /// <summary>
        /// An unhanlded exception is raised while executing a handler
        /// </summary>
        UnhandledException = -1,
    }
}
