SELECT 
    f.[FriendId],
    f.[FirstName],
    f.[LastName],
    f.[Email],
    f.[Birthday],
    a.[AddressId],
    a.[StreetAddress],
    a.[City],
    a.[ZipCode],
    p.[PetId],
    q.[QuoteId]
FROM 
    [goodfriendsefc].[supusr].[Friends] f
LEFT JOIN 
    [goodfriendsefc].[supusr].[Addresses] a
ON 
    f.[AddressId] = a.[AddressId]
LEFT JOIN 
    [goodfriendsefc].[supusr].[Pets] p
ON 
    f.[FriendId] = p.[FriendId]
LEFT JOIN 
    [goodfriendsefc].[supusr].[FriendDbMQuoteDbM] fq
ON 
    f.[FriendId] = fq.[FriendsDbMFriendId]
LEFT JOIN 
    [goodfriendsefc].[supusr].[Quotes] q
ON 
    fq.[QuotesDbMQuoteId] = q.[QuoteId]
WHERE 
    f.[FriendId] = 'cc2d5cf6-b506-4a64-8b67-6d4073f763b9';
