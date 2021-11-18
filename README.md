# C# Extra Life API Client [![Build](https://github.com/walkerboh/ExtraLife/actions/workflows/dotnet.yml/badge.svg)](https://github.com/walkerboh/ExtraLife/actions/workflows/dotnet.yml)

This C# library provides easy access to the common API endpoints you need for an integration to the Extra Life donations.

## ExtraLifeApiClient Usage

| Method                   | Use                                                              |
| ------------------------ | ---------------------------------------------------------------- |
| GetParticipants          | Returns a paged list of participants from the event.             |
| GetParticipant           | Returns a specific event participant.                            |
| GetParticipantDonations  | Returns a paged list of donations for the specified participant. |
| GetParticipantDonors     | Returns a paged list of donors for the specified participant.    |
| GetParticipantActivities | Returns the most recent activity for a specificed participant.   |

### Common Parameters

- Page: 1-indexed page number for paginated results
- Limit: Count of items to be returned by a paginated query

## Installation

As a Nuget package

```c#
Install-Package ExtraLife -Version 1.0.0

// or using the .NET CLI
dotnet add package ExtraLife --version 1.0.0
```
