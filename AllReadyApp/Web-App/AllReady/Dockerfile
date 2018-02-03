FROM microsoft/aspnetcore-build:2.0
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ALLREADY_DOCKER_DEBUG=TRUE

#Install debugger
RUN apt-get update
RUN apt-get install curl -y unzip
RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg

# Install npm packages
WORKDIR /app/Web-App/AllReady
COPY Web-App/AllReady/package.json .
COPY Web-App/AllReady/package-lock.json .
RUN ["npm", "install"]

# Run dotnet restore
WORKDIR /app
COPY *.sln ./
COPY Web-App/AllReady/AllReady.csproj Web-App/AllReady/
COPY AllReady.Core/AllReady.Core.csproj AllReady.Core/
RUN dotnet restore AllReadyWebOnly.sln
COPY . .
WORKDIR /app/Web-App/AllReady

# Build project
RUN ["dotnet","build"]

# Run gulp tasks
RUN gulp clean && gulp min
EXPOSE 80/tcp
RUN chmod +x ../../entrypoint.sh
CMD /bin/bash ../../entrypoint.sh