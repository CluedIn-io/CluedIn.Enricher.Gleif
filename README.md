# CluedIn.CSharp.Template

Template containing common files and folder structure for creating new C# repositories.

## Simple Usage

1. To create a Git repository for a new C# project follow the steps below

    ```Shell
    # Clone the CSharp template repository and step into the new folder
    git clone https://github.com/CluedIn-io/CluedIn.CSharp.Template.git CluedIn.ProjectName
    cd CluedIn.ProjectName

    # Rename the Git remote to template
    git remote rename origin template
    ```

1. Rename the Visual Studio solution in the root folder

    ```Shell
    ren CluedIn.CSharp.Template.sln CluedIn.ProjectName.sln
    ```

1. Create the CluedIn.ProjectName repository under the CluedIn organization in GitHub

1. Set the new GitHub repository as the _origin_ remote

    ```Shell
    git remote add origin https://github.com/CluedIn-io/CluedIn.ProjectName
    ```

1. Push the content to the new Git repository

    ```Shell
    git push
    ```

Substitute _CluedIn.ProjectName_ for your project name.

## CluedIn Repository Code Migration

To use this repository as a basis for migrating out an existing project from another repository use the instructions provided below.

1. Create a feature branch to perform the code migration in

    ```Shell
    git checkout feature/Migrate-CluedIn-Repository-Code
    ```

1. Copy across code from source repository

1. Convert projects to VS2017 format so that `dotnet` CLI can work with them

    ```PowerShell
    gci . *.csproj -recurse | ForEach-Object { dotnet migrate-2017 migrate $_.FullName }
    ```

1. Remove backup files

    ```PowerShell
    gci . backup* -Directory -recurse | remove-item -recurse -force -verbose
    ```

# About CluedIn
CluedIn is the Cloud-native Master Data Management Platform that brings data teams together enabling them to deliver the foundation of high-quality, trusted data that empowers everyone to make a difference. 

We're different because we use enhanced data management techniques like [Graph](https://www.cluedin.com/graph-versus-relational-databases-which-is-best) and [Zero Upfront Modelling](https://www.cluedin.com/upfront-versus-dynamic-data-modelling) to accelerate the time taken to prepare data to deliver insight by as much as 80%. Installed in as little as 20 minutes from the [Azure Marketplace](https://azuremarketplace.microsoft.com/en-gb/marketplace/apps/cluedin.azure_cluedin?tab=Overview), CluedIn is fully integrated with [Microsoft Purview](https://www.cluedin.com/product/microsoft-purview-mdm-integration?hsCtaTracking=461021ab-7a38-41a3-93dd-cfe2325dfd35%7Cb835efc0-e9b7-4385-a1b6-75cb7632527b) and the full [Microsoft Fabric](https://www.cluedin.com/microsoft-fabric) suite, making it the preferred choice for [Azure customers](https://www.cluedin.com/microsoft-intelligent-data-platform). 

To learn more about CluedIn, [contact the team](https://www.cluedin.com/discovery-call) today.

[https://www.cluedin.com](https://www.cluedin.com)
