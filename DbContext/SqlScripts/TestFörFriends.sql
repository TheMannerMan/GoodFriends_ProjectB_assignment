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
    p.[Name] AS PetName,
    p.[Kind] AS PetKind,
    p.[Mood] AS PetMood,
    q.[QuoteId],
    q.[QuoteText],
    q.[Author]
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
    f.[FriendId] = '1476c7c3-d6e1-406b-923e-2243df7eb3ff';
