﻿namespace Task2;

public interface ILibraryOperationService
{
    void BeginOperation(Guid requestId, RequestModel model, CancellationToken cancellationToken);
}